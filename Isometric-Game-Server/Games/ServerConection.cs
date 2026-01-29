using Isometric_Game_Server.Data;
using LiteNetLib;
using System;

namespace Isometric_Game_Server.Games {
    public class ServerConection {
        public int ConnectionId { get; set; }
        public User User { get; set; }

        public NetPeer Peer { get; set; }

        public Guid? GameId { get; set; }
    }
}
