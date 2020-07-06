using FeedbackService.DataAccess.Models;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Managers.Interfaces
{
    public interface IFeedbackManager
    {
        Task<Feedback> CreateAsync(long userId, long orderId, Feedback feedback, CancellationToken cancellationToken);
        Task<Feedback> GetAsync(long userId, long orderId, CancellationToken cancellationToken);
        Task UpdateAsync(long orderId, int rating, string comment, CancellationToken cancellationToken);
        Task DeleteAsync(long orderId, CancellationToken cancellationToken);
    }
}
