﻿using book.DataAccess.Data;
using book.DataAccess.Repository.IRepository;
using book.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace book.DataAccess.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext _context;
        public CoverTypeRepository (ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public void Upadate(CoverType coverType)
        {
            _context.Update(coverType);
        }
    }
}
