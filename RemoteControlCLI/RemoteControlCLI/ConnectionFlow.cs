using System;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Text;

namespace RemoteControlCLI
{
    class ConnectionFlow
    {
        private readonly IUserInterface _userInterface;

        public ConnectionFlow(IUserInterface userInterface)
        {
            _userInterface = userInterface;
        }

        public IObservable<NetActor> Connect()
             => _userInterface
                .RequestInput(Constants.EnterHostName)
                .SelectMany(hostname =>
                    _userInterface
                        .RequestInput(Constants.EnterPortNumber)
                        .Select(port =>
                            new NetActor(hostname, int.Parse(port))))
                        .Retry(_userInterface);
    }
}
