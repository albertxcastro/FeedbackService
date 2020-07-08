using FeedbackService.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Managers.Interfaces
{
    public interface IProductFeedbackManager
    {
        Task<Feedback> CreateFeedbackAsync(Feedback feedback, long orderId, long productId, CancellationToken cancellationToken);
        Task DeleteFeedbackAsync(long orderId, long productId, CancellationToken cancellationToken);
    }
}
