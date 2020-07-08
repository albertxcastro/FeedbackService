using FeedbackService.DataAccess.Models;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Facade.Interfaces
{
    public interface IProductFeedbackFacade
    {
        Task<Feedback> CreateAsync(long userId, long orderId, long productId, Feedback feedback, CancellationToken cancellationToken);
        Task<Feedback> GetAsync(long userId, long orderId, long productId, CancellationToken cancellationToken);
        Task<Feedback> UpdateAsync(long userId, long orderId, long productId, Feedback feedback, CancellationToken cancellationToken);
        Task DeleteAsync(long userId, long orderId, long productId, CancellationToken cancellationToken);
    }
}
