using Project.Core.Interfaces;
using Project.Data;
using Project.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core
{
    public abstract class Service<TEntity> : IService<TEntity> where TEntity : class
    {
        protected internal IRepository<TEntity> repository;

        public Service()
        {
            repository = new RepositoryFactory().Create<TEntity>();
        }

        public virtual bool Insert(TEntity entity)
        {
            return repository.Insert(entity);
        }

        public virtual bool Update<Tkey>(TEntity entity, Tkey id)
        {
            return repository.Update(entity,id);
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return repository.GetAll();
        }

        public virtual TEntity Get<Tkey>(Tkey id)
        {
            return repository.Get(id);
        }
    }
}
