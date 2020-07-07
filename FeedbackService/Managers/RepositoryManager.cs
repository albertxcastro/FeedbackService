using CachingManager.Managers;
using FeedbackService.DataAccess.Context;
using FeedbackService.DataAccess.Models;
using FeedbackService.Managers.Interfaces;
using FeedbackService.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Managers
{
    public class RepositoryManager : IRepository
    {
        private readonly DataContext _dbContext;

        public RepositoryManager(DataContext context)
        {
            _dbContext = context;
        }

        public async Task<T> GetAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken) where T : BaseEntity
        {
            var dbSet = _dbContext.Set<T>();
            return await dbSet.Where(predicate).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken) where T : BaseEntity
        {
            var dbSet = _dbContext.Set<T>();
            return await dbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<List<T>> GetAllAsync<T>(CancellationToken cancellationToken) where T : BaseEntity
        {
            var dbSet = _dbContext.Set<T>();
            return await dbSet.ToListAsync(cancellationToken);
        }

        public async Task<T> CreateAsync<T>(T entity, CancellationToken cancellationToken) where T : BaseEntity
        {
            using (IDbContextTransaction transaction = _dbContext.Database.BeginTransaction())
            {
                var dbSet = _dbContext.Set<T>();
                await dbSet.AddAsync(entity, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                transaction.Commit();
            }

            return entity;
        }

        public void Update<T>(T entity) where T : BaseEntity
        {
            _dbContext.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete<T>(T entity) where T : BaseEntity
        {
            var dbSet = _dbContext.Set<T>();
            dbSet.Remove(entity);
            _dbContext.SaveChanges();
        }
    }
}
