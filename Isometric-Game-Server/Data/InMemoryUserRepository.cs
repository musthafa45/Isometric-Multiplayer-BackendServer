
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Isometric_Game_Server.Data {
    public class InMemoryUserRepository : IUserRepository {

        //Real case scenario we would have a database connection here
        private readonly List<User> _entities;
        private readonly ILogger<InMemoryUserRepository> logger;

        public InMemoryUserRepository(ILogger<InMemoryUserRepository> logger) {
            _entities = new List<User>{
                new User {
                    Id="Riyaz", Password="qqq", Score=100, IsOnline = false
                },
                new User {
                    Id="Riyas", Password="qqq", Score=200, IsOnline = false
                },
                new User {
                    Id="Musthafa", Password="qqq", Score=300, IsOnline = true
                }
            };

            this.logger = logger;
        }

        public void Add(User entity) {
            _entities.Add(entity);
        }

        public void Delete(User entity) {
             User entityToDelete = _entities.FirstOrDefault(e => e.Id == entity.Id);
            _entities.Remove(entityToDelete);
        }

        public User Get(string Id) {
            User user = _entities.FirstOrDefault(e => e.Id == Id);
            if (user != null) {
                return user;
            }
            else {
                logger.LogError("User not found on: {Id} Id", Id);
                return null;
            }
        }

        public IQueryable<User> GetQuery() {
            return _entities.AsQueryable();
        }

        public ushort GetTotalCount() {
            return (ushort)_entities.Count(x => x.IsOnline);
        }

        public void SetOnline(string id, bool online) {
            User user = _entities.FirstOrDefault(e => e.Id == id);
            if (user != null) {
                user.IsOnline = online;
            }
            else {
                logger.LogError("User not found: {Id}", id);
            }
        }

        public void Update(User entity) {
            int entityIndex = _entities.FindIndex(e => e.Id == entity.Id);
            _entities[entityIndex] = entity;
        }
    }
}
