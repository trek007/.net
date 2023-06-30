﻿using book.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace book.DataAccess.Repository.IRepository
{
  public  interface ICampanyRepository:IRepository<Campany>
    {
        void Update(Campany campany);
    }
}
