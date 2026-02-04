using Isometric_Game_Server.Data;
using Isometric_Game_Server.Games;
using NetworkShared;
using NetworkShared.Attributes;
using NetworkShared.Packets.ServerClient;

namespace Isometric_Game_Server.PacketHandlers {

    [HandlerRegister(PacketType.ServerRequestStatus)]
    public class ServerStatusRequestHandler : IPacketHandler {
        private readonly NetworkServer networkServer;
        private readonly IUserRepository userRepository;
        private readonly UsersManager usersManager;

        public ServerStatusRequestHandler(NetworkServer networkServer,IUserRepository userRepository,UsersManager usersManager) {
            this.networkServer = networkServer;
            this.userRepository = userRepository;
            this.usersManager = usersManager;
        }
        public void HandlePacket(INetPacket packet, int connectionId) {

            Net_OnServerStatus response = new Net_OnServerStatus {
               OnlinePlayersCount = userRepository.GetTotalOnlinePlayerCount(),
               TopPlayersNetDTOs = usersManager.GetTopPlayersDTOs()
            };

            networkServer.SendPacketToClient(response, connectionId);
        }
    }
}
