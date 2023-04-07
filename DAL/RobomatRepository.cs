using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IGenericRepository<T>
            where T : class
    {
        T FindById(object EntityId);
        IEnumerable<T> Select(Expression<Func<T, bool>> Filter = null);
        void Insert(T Entity);
        void Update(T Entity);
        void Delete(object EntityId);
        void Delete(T Entity);
    }

    public class RobomatRepository<T>
        : IGenericRepository<T>
        where T : class
    {
        private RobomatContext _context;
        private DbSet<T> _dbSet;
        public RobomatRepository(RobomatContext Context)
        {
            _context = Context;
            _dbSet = _context.Set<T>();
        }
        public virtual T FindById(object EntityId)
        {
            return _dbSet.Find(EntityId);
        }

        public virtual IList<T> GetAll(params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> dbQuery = _dbSet;
            foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                dbQuery = dbQuery.Include<T, object>(navigationProperty);

            return dbQuery.AsNoTracking().ToList<T>();
        }

        public virtual IEnumerable<T> Select(Expression<Func<T, bool>> Filter = null)
        {
            if (Filter != null)
            {
                return _dbSet.Where(Filter);
            }
            return _dbSet;
        }
        public virtual void Insert(T entity)
        {
            _dbSet.Add(entity);
        }
        public virtual void Update(T entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
            _context.SaveChanges();
        }
        public virtual void Delete(object EntityId)
        {
            T entityToDelete = _dbSet.Find(EntityId);
            Delete(entityToDelete);
        }
        public virtual void Delete(T Entity)
        {
            if (_context.Entry(Entity).State == EntityState.Detached) //Concurrency için 
            {
                _dbSet.Attach(Entity);
            }
            _dbSet.Remove(Entity);
        }
    }
}
