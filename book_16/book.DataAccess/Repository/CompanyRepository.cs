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
    public class CompanyRepository : Repository<Campany>, ICampanyRepository
    {
        private readonly ApplicationDbContext _context;
        public CompanyRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public void Update(Campany campany)
        {
            _context.Update(campany);
        }
    }
}
