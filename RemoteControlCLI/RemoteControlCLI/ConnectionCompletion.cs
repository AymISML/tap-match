using System;
using System.Threading;

namespace RemoteControlCLI
{
    class ConnectionCompletion
    {
        private readonly AutoResetEvent _resetEvent = new AutoResetEvent(false);
        private readonly IGame _gameFlow;

        public ConnectionCompletion(GameFlow gameFlow)
        {
            _gameFlow = gameFlow;
        }

        public void WaitForCompleted() => _resetEvent.WaitOne();

        public void CompleteWithSuccess(NetActor netActor)
        {
            Console.WriteLine("Connected!");
            _gameFlow.Run(netActor);
        }

        public void CompleteWithError(Exception e)
        {
            Console.WriteLine($"Failed: {e}");
            _resetEvent.Set();
        }
    }
}
