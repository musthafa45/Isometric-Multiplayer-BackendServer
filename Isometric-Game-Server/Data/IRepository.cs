using System.Linq;

namespace Isometric_Game_Server.Data {
    public interface IRepository<T> where T : class {
        void Add(T entity);

        void Delete(T entity);

        void Update(T entity);  

        T Get(string id);

        IQueryable<T> GetQuery();

        ushort GetTotalCount();
    }
}
