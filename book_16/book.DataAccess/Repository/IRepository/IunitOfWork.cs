using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace book.DataAccess.Repository.IRepository
{
   public interface IunitOfWork
    {
        ICategoryRepository Category { get; }
        ICoverTypeRepository CoverType { get; }
        IProductRepository Product { get; set; }
        ICampanyRepository Campany { get; set; }
        IApplicationUserRepository ApplicationUser { get; set; }
        IShoppingCartRepository ShoppingCart { get; set; }
        IOrderDetailRepository OrderDetail { get; set; }
        IOrderHeaderRepository OrderHeader { get; set; }
        void Save();
    }
}
