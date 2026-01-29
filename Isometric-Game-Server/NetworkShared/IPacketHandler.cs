namespace Isometric_Game_Server.NetworkShared {
    public interface IPacketHandler {
        void HandlePacket(INetPacket packet, int connectionId);
    }
}
