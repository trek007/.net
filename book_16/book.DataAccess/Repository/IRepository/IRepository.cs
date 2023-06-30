
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace book.DataAccess.Repository.IRepository
{
  public  interface IRepository<T> where T:class
    {
        T Get(int id);
        //find
        IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedEnumerable<T>> orderBy = null,
            string includeProperties = null
            );
        T FirstorDefault(
             Expression<Func<T, bool>> filter = null,
             string includeProperties = null                 //category,covertype
            );
        void Add(T entity);
        void remove(T entity);
        void remove(int id);
        void remove(IEnumerable<T> entity);
    }
}
