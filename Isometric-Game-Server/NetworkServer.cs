using Isometric_Game_Server.NetworkShared;
using Isometric_Game_Server.NetworkShared.Registries;
using LiteNetLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetworkShared;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Isometric_Game_Server {
    public class NetworkServer : INetEventListener {
        private NetManager netManager;
        private Dictionary<int, NetPeer> connections;

        private readonly ILogger<NetworkServer> Logger;
        private readonly IServiceProvider ServiceProvider;

        public NetworkServer(ILogger<NetworkServer> logger, IServiceProvider serviceProvider) {
            Logger = logger;
            ServiceProvider = serviceProvider;
        }

        public void Start() {
            connections = new Dictionary<int, NetPeer>();

            netManager = new NetManager(this) {
                DisconnectTimeout = 100000
            };

            netManager.Start(9050);

            Console.WriteLine("Server started on port 9050");
        }

        public void PollEvents() {
            netManager.PollEvents();
        }

        public void OnConnectionRequest(ConnectionRequest request) {
            Console.WriteLine("Connection request from " + request.RemoteEndPoint);
            request.Accept();
        }


        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod) {

            using(IServiceScope scope = ServiceProvider.CreateScope()) {
                try {
                    PacketType packetType = (PacketType)reader.GetByte();
                    INetPacket packet = ResolvePacket(packetType, reader);
                    IPacketHandler handler = ResolveHandler(packetType);

                    handler.HandlePacket(packet, peer.Id);
                    reader.Recycle();
                }
                catch(Exception ex) {
                    Logger.LogError(ex, "Error processing network message from peer {PeerId}", peer.Id);
                }
            }

            //var data = System.Text.Encoding.UTF8.GetString(reader.RawData);
            //Console.WriteLine("Received data from " + peer.Address + ": " + data);
            //SendReply(peer);
        }

        private void SendReply(NetPeer netPeer) {
            var message = "Hello from server";
            var data = System.Text.Encoding.UTF8.GetBytes(message);
            netPeer.Send(data, DeliveryMethod.ReliableOrdered);
        }

        public void OnPeerConnected(NetPeer peer) {
            Console.WriteLine("Peer connected: " + peer.Address + "Id " + peer.Id);
            connections.Add(peer.Id,peer);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
            Console.WriteLine("Peer disconnected: " + peer.Address + "Id " + peer.Id);
            connections.Remove(peer.Id);
        }
        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError) {
            //throw new NotImplementedException();
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) {
            //throw new NotImplementedException();
        }
        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) {
            //throw new NotImplementedException();
        }

        private IPacketHandler ResolveHandler(PacketType packetType) {
            var registry = ServiceProvider.GetRequiredService<HandlerRegistry>();
            var handlerType = registry.PacketHandlers[packetType];
            var handler = (IPacketHandler)ServiceProvider.GetRequiredService(handlerType);
            return handler;
        }

        private INetPacket ResolvePacket(PacketType packetType, NetPacketReader reader) {
            PacketRegistry registry = ServiceProvider.GetRequiredService<PacketRegistry>();
            Type type = registry.PacketTypes[packetType];
            INetPacket packet = (INetPacket)Activator.CreateInstance(type);
            packet.Deserialize(reader);
            return packet;
        }
    }
}
