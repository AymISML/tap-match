using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;

public class TcpInput : MonoBehaviour
{
    private const int PORT = 3000;

    [SerializeField] private InputEventSO inputEvent;
    [SerializeField] private GameStateEventSO gameStateEvent;

    private TcpListener tcpListener;
    private TcpClient tcpClient;
    private NetworkStream networkStream;

    private byte[] buffer = new byte[1024];

    private void Start()
    {
        IPHostEntry ipHostInfo = Dns.GetHostEntry("127.0.0.1");
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint ipEndPoint = new(ipAddress, PORT);

        tcpListener = new TcpListener(ipEndPoint);
        tcpListener.Start(100);

        if (tcpListener.Pending())
        {
            AcceptClient();
        }

        Debug.Log($"Listening on Port: {PORT}");

        gameStateEvent.OnGameStateUpdated += PublishGameState;
    }

    private void AcceptClient()
    {
        tcpClient = tcpListener.AcceptTcpClient();
        networkStream = tcpClient.GetStream();

        gameStateEvent.RaiseRefreshGameState();
        Debug.Log($"Accepted client: {tcpClient.Client}");
    }

    private void Update()
    {
        if (tcpClient == null && tcpListener.Pending())
        {
            AcceptClient();
        }

        if (tcpClient == null)
            return;

        if (networkStream == null || !networkStream.DataAvailable)
            return;

        ReadFromStream();
    }

    private void OnDestroy()
    {
        gameStateEvent.OnGameStateUpdated -= PublishGameState;
        tcpListener.Stop();
    }

    public void PublishGameState(GameState gameState)
    {
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(gameState);
        Send(json);
    }

    private void ReadFromStream()
    {
        try
        {
            var length = networkStream.Read(buffer);
            var data = new byte[length];
            Array.Copy(buffer, 0, data, 0, length);
            string message = Encoding.Default.GetString(data);

            if (TryParseCoordinates(message, out Vector2Int input))
                UnityMainThreadDispatcher
                    .Instance()
                    .Enqueue(() => inputEvent.RaiseTileTapped(input));
            else
                Send("Ex: failed, coordinates provided couldn't be parsed!");
        }
        catch (SocketException e)
        {
            Debug.LogError(e);
        }
    }

    private void Send(string message)
    {
        if (tcpClient == null || !tcpClient.Connected)
            return;

        try
        {
            var bytes = Encoding.Default.GetBytes(message);
            networkStream.Write(bytes);
        }
        catch (SocketException e)
        {
            Debug.LogError(e);
        }
    }

    private bool TryParseCoordinates(string message, out Vector2Int coordinates)
    {
        coordinates = Vector2Int.zero;

        var split = message.Split(',');
        if (split.Length != 2)
            return false;

        for (int i = 0; i < split.Length; i++)
        {
            var coord = split[i].Split('=');
            if (coord.Length != 2)
                return false;

            if (int.TryParse(coord[1], out var value))
            {
                switch (coord[0].Trim().ToLower())
                {
                    case "x":
                        coordinates.x = value;
                        break;
                    case "y":
                        coordinates.y = value;
                        break;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }
}
