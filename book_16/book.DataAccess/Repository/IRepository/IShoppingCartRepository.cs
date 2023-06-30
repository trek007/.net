using book.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace book.DataAccess.Repository.IRepository
{
   public interface IShoppingCartRepository:IRepository<ShoppingCart>
    {
        void Update(ShoppingCart shoppingCart);
        //void RemoveRange(IEnumerable<ShoppingCart> listCart);
    }
}
