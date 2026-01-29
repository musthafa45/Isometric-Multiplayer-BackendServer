using Isometric_Game_Server.Games;
using Isometric_Game_Server.NetworkShared;
using Microsoft.Extensions.Logging;
using NetworkShared;
using NetworkShared.Attributes;
using NetworkShared.Packets.ClientServer;

namespace Isometric_Game_Server.PacketHandlers {

    [HandlerRegister(PacketType.AuthRequest)]
    public class AuthRequestHandler : IPacketHandler {
        private readonly ILogger<AuthRequestHandler> logger;
        private readonly UsersManager usersManager;

        public AuthRequestHandler(ILogger<AuthRequestHandler> logger,UsersManager usersManager) {
            this.logger = logger;
            this.usersManager = usersManager;
        }
        public void HandlePacket(INetPacket packet, int connectionId) {

            Net_AuthRequest msg = (Net_AuthRequest)packet;

            logger.LogInformation($"Received AuthRequest from ConnectionId {connectionId} with Username: {msg.Username} And Password {msg.Password}");

            bool loginSuccess = usersManager.TryAuthenticateUser(connectionId,msg.Username, msg.Password);
            // logging 
            // Loging or register
            // if Success send back AuthResponse with success
            // else send back AuthResponse with failure
        }
    }
}
