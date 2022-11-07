using System;

namespace RemoteControlCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var userInterface = new ConsoleInterface();

            var completion = new ConnectionCompletion(new GameFlow(userInterface));
            new ConnectionFlow(userInterface)
                .Connect()
                .Subscribe(completion.CompleteWithSuccess, completion.CompleteWithError);
            completion.WaitForCompleted();
        }
    }
}