using Isometric_Game_Server.Data;
using LiteNetLib;
using NetworkShared.Packets.ServerClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Isometric_Game_Server.Games {
    public class UsersManager {
        private Dictionary<int, ServerConection> _connections = new Dictionary<int , ServerConection>();
        private readonly IUserRepository userRepository;

        public UsersManager(IUserRepository userRepository) {
            this.userRepository = userRepository;
        }

        public void AddConnection(NetPeer peer) {
            _connections.Add(peer.Id, new ServerConection {
                ConnectionId = peer.Id,
                Peer = peer,
            });

            Console.WriteLine($"New connection added: {peer.Id}");
        }

        public void DisconnectConnection(int peerId) {
           ServerConection serverConection = GetConnection(peerId);
           if(serverConection.User != null) {
                 userRepository.SetOnline(serverConection.User.Id, false);

                //MatchMaker.UnregisterPlayer(serverConection.User.Id
                //Close Game if needed
           }
           _connections.Remove(peerId);
        }

        public ServerConection GetConnection(int connectionId) {
            if(_connections.ContainsKey(connectionId)) {
                return _connections[connectionId];
            }
            return null;
            // throw new Exception("Connection not found");
        }

        public bool TryAuthenticateUser(int connectionId,string username, string password) {
            var dbUser = userRepository.Get(username);
            if (dbUser != null) {
                 
                if(dbUser.Password != password) {
                    // same Username but wrong password
                    return false;
                }
                
            }
            else {
                // Create new user
               var newUser = new User {
                    Id = username,
                    Password = password,
                    Score = 0,
                    IsOnline = true
                };
               
                userRepository.Add(newUser);

                dbUser = newUser;
            }

            if(_connections.ContainsKey(connectionId)) {
                // User is already connected
                dbUser.IsOnline = true;
                _connections[connectionId].User = dbUser;
            }

            return true;
        }

        public int[] GetAllConnectedPeersIdExcluding(int connectionId) {
            return _connections.Keys.Where(id => id != connectionId).ToArray();
        }

        public PlayerNetDTO[] GetTopPlayersDTOs() {
            return userRepository.GetQuery()
                .OrderByDescending(x => x.Score)
                .Take(10)
                .Select(x => new PlayerNetDTO {
                    Username = x.Id,
                    Score = x.Score,
                    IsOnline = x.IsOnline
                })
                .ToArray();
        }

    }
}
