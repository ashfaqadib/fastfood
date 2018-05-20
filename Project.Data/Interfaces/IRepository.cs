using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Data.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        bool Insert(TEntity entity);
        bool Update<Tkey>(TEntity entity, Tkey id);
        IEnumerable<TEntity> GetAll();
        TEntity Get<TKey>(TKey id);
    }
}
