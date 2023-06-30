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
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderDetailsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(OrderDetails orderDetails)
        {
            _context.Update(orderDetails);
        }
    }
}
