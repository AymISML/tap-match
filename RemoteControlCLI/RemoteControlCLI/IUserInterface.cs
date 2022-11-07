using System;

namespace RemoteControlCLI
{
    public interface IUserInterface
    {
        void ShowMessage(string message);
        IDisposable ShowProgressIndicator(string operationDescription);
        IObservable<string> RequestInput(string prompt);
    }
}
