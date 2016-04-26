using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Create.CSP.GitHub.Reporting.Database.Model.Managers
{
    public class BaseManager : IDisposable
    {
        protected bool _usingInternalContext;
        protected CSPDatabaseModelEntities _dbContext;

        #region Constructor

        public BaseManager(DbContext dbContext = null)
        {
            if (dbContext == null)
            {
                // Use internal context
                _usingInternalContext = true;
                _dbContext = new CSPDatabaseModelEntities();
            }
            else
            {
                // Use received context (may contain transactions, etc)
                _usingInternalContext = false;
                _dbContext = (CSPDatabaseModelEntities)dbContext;
            }
        }

        public void Dispose()
        {
            if (_usingInternalContext)
            {
                // Dispose
                _dbContext.Dispose();
            }
            // else. Dont dispose external context
        }

        #endregion

        public TEntity AddOrUpdate<TEntity>(TEntity entity,
            Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            TEntity result;
            var dbEntity = _dbContext.Set<TEntity>().FirstOrDefault(predicate);

            if (dbEntity != null)
            {
                // Exists. Update if changed             
                _dbContext.Entry(dbEntity).CurrentValues.SetValues(entity);
                // Force modified
                _dbContext.Entry(dbEntity).State = EntityState.Modified;

                result = dbEntity;
            }
            else
            {
                // Does not exist. Create
                result = _dbContext.Set<TEntity>().Add(entity);
            }

            _dbContext.SaveChanges();
            return result;
        }

        public IEnumerable<TEntity> AddOrUpdate<TEntity>(IEnumerable<TEntity> entities,
            Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            BlockingCollection<TEntity> result = new BlockingCollection<TEntity>(entities.Count());

            entities.AsParallel().ForAll(entity =>
            {
                var dbEntity = _dbContext.Set<TEntity>().FirstOrDefault(predicate);
                if (dbEntity != null)
                {
                    // Exists. Update
                    _dbContext.Entry(dbEntity).CurrentValues.SetValues(entity);
                    result.Add(entity);
                }
                else
                {
                    // Does not exist. Create
                    result.Add(_dbContext.Set<TEntity>().Add(entity));
                }
            });

            _dbContext.SaveChanges();
            return result;
        }

        public IEnumerable<TEntity> GetAll<TEntity>(Expression<Func<TEntity, bool>> predicate = null,
            params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class
        {
            var dbEntities = _dbContext.Set<TEntity>();

            if (includeProperties != null)
            {
                foreach (var property in includeProperties)
                {
                    dbEntities.Include(property);
                }
            }

            if (predicate != null)
            {
                dbEntities.Where(predicate);
            }

            return dbEntities;
        }

        public IEnumerable<TEntity> FindAllBy<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            var dbEntities = _dbContext.Set<TEntity>().Where(predicate);

            return dbEntities;
        }

        public TEntity FirstOrDefaultBy<TEntity>(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class
        {
            TEntity result;

            var dbEntity = _dbContext.Set<TEntity>();

            if (includeProperties != null)
            {
                foreach (var property in includeProperties)
                {
                    dbEntity.Include(property);
                }
            }

            result = dbEntity.FirstOrDefault(predicate);

            return result;
        }


        public int Delete<TEntity>(TEntity entity) where TEntity : class
        {
            _dbContext.Set<TEntity>().Remove(entity);

            return _dbContext.SaveChanges();
        }

        public void DeleteWhere<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            var dbSet = _dbContext.Set<TEntity>();

            if (predicate != null)
            {
                dbSet.RemoveRange(dbSet.Where(predicate));
            }

            _dbContext.SaveChanges();
        }
    }
}
