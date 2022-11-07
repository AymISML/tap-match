using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace RemoteControlCLI
{
    public struct GameState
    {
        public int score { get; set; }
        public int[][] matchableStates { get; set; }

        public static GameState fromJson(string gameStateJson)
        {
            return JsonSerializer.Deserialize<GameState>(gameStateJson);
        }
    }
}
