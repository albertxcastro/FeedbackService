using CachingManager.Managers;
using FeedbackService.DataAccess.Context;
using FeedbackService.DataAccess.Models;
using FeedbackService.Managers.Interfaces;
using FeedbackService.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
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

        public async Task<T> CreateAsync<T>(T entity, CancellationToken cancellationToken) where T : BaseEntity
        {
            var dbSet = _dbContext.Set<T>();
            await dbSet.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return await dbSet.Where(e => entity == e).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task UpdateAsync<T>(T entity, CancellationToken cancellationToken) where T : BaseEntity
        {
            _dbContext.Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync<T>(T entity, CancellationToken cancellationToken) where T : BaseEntity
        {
            var dbSet = _dbContext.Set<T>();
            dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
