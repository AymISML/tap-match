using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace RemoteControlCLI
{
    public class NetActor : IDisposable
    {
        private TcpClient tcpClient;
        private NetworkStream networkStream;

        private ISubject<string> incoming;
        public IObservable<string> Incoming
        {
            get => incoming.AsObservable();
        }

        private ISubject<string> outgoing;
        public ISubject<string> Outgoing
        {
            get => outgoing;
        }

        public NetActor(string hostname, int port)
        {
            incoming = new Subject<string>();
            outgoing = new Subject<string>();

            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(hostname, port);
                networkStream = tcpClient.GetStream();

                var listenThread = new Thread(Listen);
                listenThread.Start();

                outgoing.Subscribe(Send);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void Listen()
        {
            try
            {
                byte[] buffer = new byte[1024];
                while (true)
                {
                    if (tcpClient == null || !tcpClient.Connected)
                        continue;

                    if (networkStream == null || !networkStream.DataAvailable)
                        continue;

                    var length = networkStream.Read(buffer);
                    var data = new byte[length];
                    Array.Copy(buffer, 0, data, 0, length);
                    string message = Encoding.Default.GetString(data);
                    incoming.OnNext(message);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Send(string data)
        {
            if (tcpClient == null || !tcpClient.Connected)
                return;

            try
            {
                var bytes = Encoding.Default.GetBytes(data);
                networkStream.Write(bytes);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Dispose()
        {
            if (tcpClient != null)
            {
                tcpClient.Dispose();
            }
        }
    }
}
