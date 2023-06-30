using book.DataAccess.Data;
using book.DataAccess.Repository.IRepository;
using book.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace book.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _context;
        public ShoppingCartRepository(ApplicationDbContext context):base(context)

        {
            _context = context;
        }

        //public void RemoveRange(IEnumerable<ShoppingCart> listCart)
        //{
        //    throw new NotImplementedException();
        //}

        public void Update(ShoppingCart shoppingCart)
        {
            _context.Update(shoppingCart);
        }
    }
}
