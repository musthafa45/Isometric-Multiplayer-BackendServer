using System;
using System.Collections.Generic;

namespace Isometric_Game_Server.Games {
    public class Game {

        public Game(string[] players) {
            Id = Guid.NewGuid();
            CurrentRound = 1;
            StartTime = DateTime.UtcNow;
            CurrentRoundStartTime = DateTime.UtcNow;

            PlayersInGameData = new List<PlayerIngameData>();
            foreach (string player in players) {
                PlayersInGameData.Add(new PlayerIngameData {
                    Player = player,
                    WinCount = 0,
                    IsWantRematch = false
                });
            }

            CurrentUserTurn = players[0];
        }
        public Guid Id { get; set; }

        public ushort CurrentRound { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime CurrentRoundStartTime { get; set; }

        public List<PlayerIngameData> PlayersInGameData { get; set; }

        public string CurrentUserTurn { get; set; }
    }

    public class PlayerIngameData {
        public string Player { get; set; }
        public ushort WinCount { get; set; }
        public bool IsWantRematch { get; set; }
    }
}
