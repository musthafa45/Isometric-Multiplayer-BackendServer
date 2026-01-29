using Isometric_Game_Server.Data;
using System.Collections.Generic;

namespace Isometric_Game_Server.Games {
    public class UsersManager {
        private Dictionary<int, ServerConection> _connections = new Dictionary<int , ServerConection>();
        private readonly IUserRepository userRepository;

        public UsersManager(IUserRepository userRepository) {
            this.userRepository = userRepository;
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
               
                dbUser = newUser;
                userRepository.Add(dbUser);
            }

            if(_connections.ContainsKey(connectionId)) {
                // User is already connected
                dbUser.IsOnline = true;
                _connections[connectionId].User = dbUser;
            }

            return true;
        }
    }
}
