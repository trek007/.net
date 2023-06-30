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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
      
       public void Update(Product product)
        {
            var productInDb = _context.Products.FirstOrDefault(p => p.Id == product.Id);
            if (productInDb != null)
            {
                if (product.Imageurl != "")
                {
                    productInDb.Imageurl = product.Imageurl;
                    productInDb.Title = product.Title;
                    productInDb.Discription = product.Discription;
                    productInDb.ISBN = product.ISBN;
                    productInDb.Author = product.Author;
                    productInDb.Listprice = product.Listprice;
                    productInDb.Price = product.Price;
                    productInDb.Price100 = product.Price100;
                    productInDb.Price50 = product.Price50;
                    productInDb.CategoryId = product.CategoryId;
                    productInDb.CoverTypeId = product.CoverTypeId;
                }
            }
        }
        
    }
}
