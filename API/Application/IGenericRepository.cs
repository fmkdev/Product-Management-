using System.Linq.Expressions;

namespace API.Application
{
    public interface IGenericRepository<TEntity>
        where TEntity : class
    {


        Task<TEntity> Get(Expression<Func<TEntity, bool>> where);
        Task<TEntity> GetById(int id);
        IQueryable<TEntity> Query();

        Task Create(TEntity entity);

        void Add(TEntity entity);

        Task Update(TEntity entity);

        Task Delete(TEntity entity);
        Task Delete(List<TEntity> entity);
        Task<bool> Save();
    }
}
