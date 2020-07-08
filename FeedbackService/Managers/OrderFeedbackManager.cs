using FeedbackService.DataAccess.Models;
using FeedbackService.Managers.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Managers
{
    public class OrderFeedbackManager : BaseManager, IOrderFeedbackManager
    {
        public async Task<Feedback> CreateFeedbackAsync(Order order, Feedback feedback, CancellationToken cancellationToken)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    await _dbContext.Feedback.AddAsync(feedback, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    order.FeedbackSid = feedback.Sid;
                    _dbContext.Update(order);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

            return feedback;
        }

        public async Task DeleteFeedbackAsync(Order order, Feedback feedback, CancellationToken cancellationToken)
        {
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                order.FeedbackSid = null;
                _dbContext.Update(order);
                await _dbContext.SaveChangesAsync(cancellationToken);
                _dbContext.Feedback.Remove(feedback);
                await _dbContext.SaveChangesAsync(cancellationToken);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
        }
    }
}
