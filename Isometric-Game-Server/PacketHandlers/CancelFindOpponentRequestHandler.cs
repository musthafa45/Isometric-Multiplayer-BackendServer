using Isometric_Game_Server.Games;
using Isometric_Game_Server.Matchmaking;
using NetworkShared;
using NetworkShared.Attributes;
using System;


namespace PacketHandlers {
    [HandlerRegister(PacketType.CancelFindOpponentRequest)]
    public class CancelFindOpponentRequestHandler : IPacketHandler {
        private readonly UsersManager usersManager;
        private readonly Matchmaker matchmaker;

        public CancelFindOpponentRequestHandler(UsersManager usersManager, Matchmaker matchmaker) {
            this.usersManager = usersManager;
            this.matchmaker = matchmaker;
        }
        public void HandlePacket(INetPacket packet, int connectionId) {
            Console.WriteLine($"Received Cancel FindOpponentRequest from connection {connectionId}");
            ServerConection serverConection = usersManager.GetConnection(connectionId);
            matchmaker.UnregisterPlayer(serverConection.User.Id);
        }
    }
}
