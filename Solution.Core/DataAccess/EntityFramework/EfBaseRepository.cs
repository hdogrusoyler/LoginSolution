using Solution.Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Core.DataAccess.EntityFramework
{
    public class EfBaseRepository<TEntity, TContext> : IBaseRepository<TEntity>
        where TEntity : class, IEntity, new()
        where TContext : DbContext
    {
        private TContext _dbContext;
        public EfBaseRepository(TContext dbContext)
        {
            _dbContext = dbContext;
        }

        public TEntity Get(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();
            if (include != null)
            {
                query = include(query);
            }
            return query.SingleOrDefault(filter);
        }

        public List<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int page = 1,
            int pageSize = 0,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (include != null)
            {
                query = include(query);
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (pageSize > 0)
            {
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
            }
            return query.ToList();
        }

        public int GetListCount(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query.ToList().Count();
        }

        public TEntity Add(TEntity entity)
        {
            var addedEntity = _dbContext.Entry(entity);
            addedEntity.State = EntityState.Added;
            //_dbContext.SaveChanges();
            return entity;
        }

        public TEntity Update(TEntity entity)
        {
            var updatedEntity = _dbContext.Entry(entity);
            updatedEntity.State = EntityState.Modified;
            //_dbContext.SaveChanges();
            return entity;
        }

        public TEntity Delete(TEntity entity)
        {
            var deleteEntity = _dbContext.Entry(entity);
            deleteEntity.State = EntityState.Deleted;
            //_dbContext.SaveChanges();
            return entity;
        }

    }
}
