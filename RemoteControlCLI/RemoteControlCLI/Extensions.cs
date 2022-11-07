using System;
using System.Reactive.Linq;

namespace RemoteControlCLI
{
    public static class Extensions
    {
        public static IObservable<T> Retry<T>(
            this IObservable<T> observable,
            IUserInterface userInterface)
            => observable
                .Do(
                    _ => { },
                    e => userInterface.ShowMessage(e.Message))
                .Retry();
    }
}
