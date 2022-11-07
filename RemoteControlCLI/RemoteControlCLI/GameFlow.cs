using System;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Text;

namespace RemoteControlCLI
{
    public class GameFlow : IGame
    {
        private readonly IUserInterface _userInterface;

        public GameFlow(IUserInterface userInterface)
        {
            _userInterface = userInterface;
        }

        public void Run(NetActor netActor)
        {
            netActor.Incoming
                .Subscribe(incomingJson =>
                {
                    if (incomingJson.StartsWith("Ex:"))
                        _userInterface.ShowMessage(incomingJson);
                    else
                        DisplayGame(incomingJson);

                    netActor
                        .Outgoing
                        .OnNext(
                            _userInterface.RequestInput(Constants.TapTile)
                                           .Select(tileCoordinates => tileCoordinates)
                                           .Wait()
                        );
                });
        }

        private void DisplayGame(string gameJson)
        {
            Console.Clear();

            GameState gameState = GameState.fromJson(gameJson);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Score = {gameState.score}");

            sb.Append('\t');
            for (int i = 0; i < gameState.matchableStates.Length; i++)
            {
                sb.Append($"x{i}\t");
            }
            sb.AppendLine();

            int x = 0;
            int y = gameState.matchableStates[0].Length - 1;
            while (x < gameState.matchableStates.Length)
            {
                if (y < 0)
                    break;

                sb.Append($"y{y}\t");

                for (int i = 0; i < gameState.matchableStates.Length; i++)
                {
                    sb.Append(gameState.matchableStates[i][y]);
                    sb.Append('\t');
                }

                sb.AppendLine();
                x++;
                y--;
            }

            _userInterface.ShowMessage(sb.ToString());
        }
    }
}