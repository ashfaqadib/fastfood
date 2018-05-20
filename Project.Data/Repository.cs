using Project.Data.Interfaces;
using Project.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Data
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        public MyDbContext dbContext = MyDbContext.GetDbContext();

        public bool Insert(TEntity entity)
        {
            dbContext.Set<TEntity>().Add(entity);
            return dbContext.SaveChanges() > 0;
        }

        public bool Update<Tkey>(TEntity entity,Tkey id)
        {
            TEntity other = dbContext.Set<TEntity>().Find(id);
            dbContext.Entry<TEntity>(other).State = EntityState.Detached;
            dbContext.Entry<TEntity>(entity).State = EntityState.Modified;
            return dbContext.SaveChanges() > 0;
        }

        public IEnumerable<TEntity> GetAll()
        {
            return dbContext.Set<TEntity>().ToList();
        }

        public TEntity Get<TKey>(TKey id)
        {
            return dbContext.Set<TEntity>().Find(id);
        }
    }
}
