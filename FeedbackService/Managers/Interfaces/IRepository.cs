using FeedbackService.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Managers.Interfaces
{
    public interface IRepository
    {
        Task<T> GetAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken) where T : BaseEntity;
        Task<T> CreateAsync<T>(T entity, CancellationToken cancellationToken) where T : BaseEntity;
        Task UpdateAsync<T>(T entity, CancellationToken cancellationToken) where T : BaseEntity;
        Task DeleteAsync<T>(T entity, CancellationToken cancellationToken) where T : BaseEntity;
    }
}
