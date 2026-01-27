using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Isometric_Game_Server {
    public class NetworkServer : INetEventListener {
        private NetManager server;
        private Dictionary<int, NetPeer> connectedPeers = new Dictionary<int, NetPeer>();
        public void Start() {
            server = new NetManager(this) {
                DisconnectTimeout = 100000
            };
            server.Start(9050);

            Console.WriteLine("Server started on port 9050");
        }

        public void PollEvents() {
            server.PollEvents();
        }

        public void OnConnectionRequest(ConnectionRequest request) {
            Console.WriteLine("Connection request from " + request.RemoteEndPoint);
            request.Accept();
        }


        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod) {
            //throw new NotImplementedException();
            var data = System.Text.Encoding.UTF8.GetString(reader.RawData);
            Console.WriteLine("Received data from " + peer.Address + ": " + data);

            SendReply(peer);
        }

        private void SendReply(NetPeer netPeer) {
            var message = "Hello from server";
            var data = System.Text.Encoding.UTF8.GetBytes(message);
            netPeer.Send(data, DeliveryMethod.ReliableOrdered);
        }

        public void OnPeerConnected(NetPeer peer) {
            Console.WriteLine("Peer connected: " + peer.Address + "Id " + peer.Id);
            connectedPeers.Add(peer.Id,peer);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
            Console.WriteLine("Peer disconnected: " + peer.Address + "Id " + peer.Id);
            connectedPeers.Remove(peer.Id);
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

    }
}
