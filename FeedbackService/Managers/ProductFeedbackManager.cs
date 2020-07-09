using FeedbackService.DataAccess.Models;
using FeedbackService.Managers.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Managers
{
    public class ProductFeedbackManager : BaseManager, IProductFeedbackManager
    {
        public async Task<Feedback> CreateFeedbackAsync(Feedback feedback, long orderId, long productId, CancellationToken cancellationToken)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var orderToProduct = await _dbContext.OrderToProduct.Where(otp => otp.Ordersid == orderId && otp.ProductSid == productId)
                        .FirstOrDefaultAsync(cancellationToken);

                    await _dbContext.Feedback.AddAsync(feedback, cancellationToken);
                    await _dbContext.SaveChangesAsync();

                    orderToProduct.FeedbackSid = feedback.Sid;
                    _dbContext.Update(orderToProduct);
                    await _dbContext.SaveChangesAsync();
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

        public async Task DeleteFeedbackAsync(long orderId, long productId, CancellationToken cancellationToken)
        {
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var orderToProduct = await _dbContext.OrderToProduct
                    .Where(otp => otp.Ordersid == orderId && otp.ProductSid == productId)
                    .FirstOrDefaultAsync(cancellationToken);

                var feedbackSid = orderToProduct.FeedbackSid;

                orderToProduct.FeedbackSid = null;
                _dbContext.Update(orderToProduct);
                await _dbContext.SaveChangesAsync(cancellationToken);

                var feedback = await _dbContext.Feedback
                    .Where(feedback => feedback.Sid == feedbackSid)
                    .FirstOrDefaultAsync(cancellationToken);

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
