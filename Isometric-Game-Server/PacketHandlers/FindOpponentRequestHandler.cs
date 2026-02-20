using Isometric_Game_Server.Games;
using Isometric_Game_Server.Matchmaking;
using NetworkShared;
using NetworkShared.Attributes;
using Packets.ClientServer;
using System;

namespace PacketHandlers {

    [HandlerRegister(PacketType.FindOpponentRequest)]
    public class FindOpponentRequestHandler : IPacketHandler {
        private readonly UsersManager usersManager;
        private readonly Matchmaker matchmaker;

        public FindOpponentRequestHandler(UsersManager usersManager,Matchmaker matchmaker) {
            this.usersManager = usersManager;
            this.matchmaker = matchmaker;
        }
        public void HandlePacket(INetPacket packet, int connectionId) {
            Console.WriteLine($"Received FindOpponentRequest from connection {connectionId}");
            Net_FindOpponentRequest msg = (Net_FindOpponentRequest)packet;
            ServerConection serverConection = usersManager.GetConnection(connectionId);
            matchmaker.RegisterPlayerToPool(serverConection,msg.PlayersCount);
        }
    }
}
