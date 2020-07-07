using FeedbackService.DataAccess.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Managers.Interfaces
{
    public interface IFeedbackManager
    {
        Task<Feedback> CreateAsync(long userId, long orderId, Feedback feedback, CancellationToken cancellationToken);
        Task<Feedback> GetAsync(long userId, long orderId, CancellationToken cancellationToken);
        Task<List<Feedback>> GetLatestAsync(int? rating, CancellationToken cancellationToken);
        Task<Feedback> UpdateAsync(long userId, long orderId, Feedback feedback, CancellationToken cancellationToken);
        Task DeleteAsync(long userId, long orderId, CancellationToken cancellationToken);
    }
}
