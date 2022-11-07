using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

namespace RemoteControlCLI
{
    public class ConsoleInterface : IUserInterface
    {
        private static readonly string[] ProgressIndicatorComponents = { "/", "|", "\\" };

        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        public IDisposable ShowProgressIndicator(string message)
        {
            ShowMessage(message);

            return new CompositeDisposable(
                Observable
                    .Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(100))
                    .Subscribe(i =>
                    {
                        Console.Write(ProgressIndicatorComponents[i % ProgressIndicatorComponents.Length]);
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    }),
                Disposable.Create(() =>
                {
                    Console.WriteLine();
                }));
        }

        public IObservable<string> RequestInput(string prompt)
        {
            return Observable.Defer(() =>
            {
                ShowMessage($"{prompt}: ");
                return Observable.Return(Console.ReadLine());
            });
        }
    }
}
