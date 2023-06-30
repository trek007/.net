﻿using book.DataAccess.Data;
using book.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace book.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbset;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            dbset = _context.Set<T>();
        }
        public void Add(T entity)
        {
            dbset.Add(entity);
        }


        public T FirstorDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null)
        {
            IQueryable<T> query = dbset;
            if (filter != null)
                query = query.Where(filter);
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.FirstOrDefault();
        }

        public T Get(int id)
        {
            return dbset.Find(id);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedEnumerable<T>> orderBy = null, string includeProperties = null)
        {
            IQueryable<T> query = dbset;
            if (filter != null)
                query = query.Where(filter);
            if (includeProperties != null)
            {
                foreach (var includeprop in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeprop);

                }
            }
            if (orderBy != null)
                return orderBy(query).ToList();
            return query.ToList();


        }

        public void remove(T entity)
        {
            dbset.Remove(entity);
        }

        public void remove(int id)
        {
            var entity = dbset.Find(id);
            dbset.Remove(entity);
            remove(entity);
        }

        public void remove(IEnumerable<T> entity)
        {
            dbset.RemoveRange(entity);
        }
    }
}
