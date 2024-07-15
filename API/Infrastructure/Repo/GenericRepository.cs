using API.Application;
using API.Infrastructure.ApplicationContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API.Infrastructure.Repo
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : class
    {
        private readonly ApplicationDbContext _dbContext;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<TEntity> Query()
        {
            return _dbContext.Set<TEntity>();
        }

        public async Task<TEntity> Get(Expression<Func<TEntity, bool>> where)
        {
            return await _dbContext.Set<TEntity>()
       .FirstOrDefaultAsync(where);
        }


        public async Task<TEntity> GetById(int id)
        {
            return await _dbContext.Set<TEntity>()
                .FindAsync(id);
        }

        public async Task Create(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
        }

        public void Add(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
        }
        // public IQueryable<TEntity> Query()
        // {
        //     return _dbContext.Set<TEntity>().AsQueryable();
        //
        // }            

        public async Task Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
        }

        public async Task Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }

        public async Task<bool> Save()
        {
            return await ((DbContext)_dbContext).SaveChangesAsync(default(CancellationToken)) >= 0;
        }

        public async Task Delete(List<TEntity> entity)
        {
            _dbContext.Set<TEntity>().RemoveRange(entity);
        }
    }
}
