using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TheRoomSimpleAPI.Interfaces;

namespace TheRoomSimpleAPI.Context
{
    public class TheRoomRepository<TEntity> where TEntity : class
    {
        internal TheRoomContext context;
        protected DbSet<TEntity> dbSet;

        public TheRoomRepository(TheRoomContext context)
        {
            this.context = context;
            dbSet = context.Set<TEntity>();
        }
        public async virtual Task<IEnumerable<TEntity>> Get(IRequest model, 
            Func<IRequest, IQueryable<TEntity>, IQueryable<TEntity>> callback = null,
            (int skip, int take)? limit = null)
        {
            IQueryable<TEntity> query = dbSet;
            query = callback.Invoke(model, query);

            if (limit.HasValue)
            {
                int listCount = query.Count();
                if (listCount > 0)
                {
                    int value = listCount <= limit.Value.take ? limit.Value.take : listCount;
                    return query.Skip(limit.Value.skip - 1).Take(value).ToList();
                }
            }
            return await query.ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "", (int skip, int take)? limit = null, Func<IQueryable<TEntity>, IQueryable<TEntity>> callback = null)
        {
            IQueryable<TEntity> query = dbSet;

            query = callback != null ? callback.Invoke(query) : query;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (limit.HasValue)
            {
                int listCount = query.Count();
                if(listCount > 0)
                {
                    int value = listCount >= limit.Value.take ? limit.Value.take : listCount;
                    return query.Skip(limit.Value.skip - 1).Take(value).ToList();
                }             
            }

            return await query.ToListAsync();
        }

        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public virtual bool Any(Expression<Func<TEntity, bool>> filter = null)
        {
            return dbSet.Any(filter);
        }

        public virtual int Count()
        {
            return dbSet.Count();
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}
