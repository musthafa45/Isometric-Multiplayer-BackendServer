
using Isometric_Game_Server.Games;
using System;

namespace Isometric_Game_Server.Matchmaking {
    public class MM_Request {
        public ServerConection ServerConection { get; set; }
        public DateTime RequestTime { get; set; }
        public bool IsMatchFound { get; set; }
        public ushort PlayersCount { get; set; } // 2, 3, or 4
    }
}
