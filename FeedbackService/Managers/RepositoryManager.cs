using CachingManager.Managers;
using FeedbackService.DataAccess.Context;
using FeedbackService.DataAccess.Models;
using FeedbackService.Factory;
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
        protected readonly DataContext _dbContext;
        private readonly FactoryManager _factory;

        public RepositoryManager(DataContext context, FactoryManager factoryManager)
        {
            _dbContext = context;
            _factory = factoryManager;
        }

        public virtual async Task<T> GetAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken) where T : BaseEntity
        {
            var dbSet = _dbContext.Set<T>();
            return await dbSet.Where(predicate).FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken) where T : BaseEntity
        {
            var dbSet = _dbContext.Set<T>();
            return await dbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        public virtual async Task<List<T>> GetAllAsync<T>(CancellationToken cancellationToken) where T : BaseEntity
        {
            var dbSet = _dbContext.Set<T>();
            return await dbSet.ToListAsync(cancellationToken);
        }

        public T GetManagerInstance<T>() where T : BaseManager, new()
        {
            return _factory.CreateManager<T>(_dbContext);
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
