namespace Isometric_Game_Server.Data {
    public interface IUserRepository : IRepository<User> {
        void SetOnline(string id,bool online);
    }
}
