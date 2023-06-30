using book.DataAccess.Data;
using book.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace book.DataAccess.Repository
{
    public class unitofWork : IunitOfWork
    {
        private readonly ApplicationDbContext _context;
        public unitofWork (ApplicationDbContext context)
        {
            _context = context;
            Category = new CategoryRepository(_context);
            CoverType = new CoverTypeRepository(_context);
            Product = new ProductRepository(_context);
            Campany = new CompanyRepository(_context);
            ApplicationUser = new ApplicationUserRepository(_context);
            ShoppingCart = new ShoppingCartRepository(_context);
            OrderHeader = new OrderHeaderRepository(_context);
            OrderDetail = new OrderDetailsRepository(_context);

        }
        public ICategoryRepository Category { get; set; }

        public ICoverTypeRepository CoverType { get; set; }
        public IProductRepository Product { get; set; }
        public ICampanyRepository Campany { get; set; }
        public IApplicationUserRepository ApplicationUser { get; set; }
        public IShoppingCartRepository ShoppingCart { get; set; }
        public IOrderDetailRepository OrderDetail { get; set; }
        public IOrderHeaderRepository OrderHeader { get; set; }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
