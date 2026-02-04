using Isometric_Game_Server.Data;
using Isometric_Game_Server.Games;
using Microsoft.Extensions.Logging;
using NetworkShared;
using NetworkShared.Attributes;
using NetworkShared.Packets.ClientServer;
using NetworkShared.Packets.ServerClient;

namespace Isometric_Game_Server.PacketHandlers {

    [HandlerRegister(PacketType.AuthRequest)]
    public class AuthRequestHandler : IPacketHandler {
        private readonly ILogger<AuthRequestHandler> logger;
        private readonly UsersManager usersManager;
        private readonly NetworkServer networkServer;
        private readonly IUserRepository userRepository;

        public AuthRequestHandler(ILogger<AuthRequestHandler> logger, IUserRepository userRepository,UsersManager usersManager,NetworkServer networkServer) {
            this.logger = logger;
            this.usersManager = usersManager;
            this.networkServer = networkServer;
            this.userRepository = userRepository;
        }
        public void HandlePacket(INetPacket packet, int connectionId) {

            Net_AuthRequest msg = (Net_AuthRequest)packet;

            logger.LogInformation($"Received AuthRequest from ConnectionId {connectionId} with Username: {msg.Username} And Password {msg.Password}");

            bool loginSuccess = usersManager.TryAuthenticateUser(connectionId,msg.Username, msg.Password);

            INetPacket authStatusMsg = null;
            if (loginSuccess) {
                authStatusMsg = new Net_OnAuthSuccess();
            }
            else {
                authStatusMsg = new Net_OnAuthFailure();
            }

            networkServer.SendPacketToClient(authStatusMsg, connectionId);

            if(loginSuccess) {
                NotifyOtherPlayers(connectionId);
            }
            
        }

        private void NotifyOtherPlayers(int connectionId) {

            var notifyMsg = new Net_OnServerStatus {
                OnlinePlayersCount = userRepository.GetTotalOnlinePlayerCount(),
                TopPlayersNetDTOs = usersManager.GetTopPlayersDTOs()
            };

            int[] allPlayersId = usersManager.GetAllConnectedPeersIdExcluding(connectionId);

            foreach (int peerId in allPlayersId) {
                networkServer.SendPacketToClient(notifyMsg, peerId);
            }
        }
    }
}
