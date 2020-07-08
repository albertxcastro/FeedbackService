using FeedbackService.DataAccess.Models;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Managers.Interfaces
{
    public interface IOrderFeedbackManager
    {
        Task<Feedback> CreateFeedbackAsync(Order order, Feedback feedback, CancellationToken cancellationToken);
        Task DeleteFeedbackAsync(Order order, Feedback feedback, CancellationToken cancellationToken);
    }
}
