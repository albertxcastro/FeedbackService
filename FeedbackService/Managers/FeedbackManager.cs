using CachingManager.Managers;
using FeedbackService.DataAccess.Models;
using FeedbackService.Managers.Interfaces;
using FeedbackService.Options;
using FeedbackService.StringConstants.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Managers
{
    public class FeedbackManager : BaseManager, IFeedbackManager
    {
        private readonly IRepository _repository;
        private readonly IOrderManager _orderManager;
        private readonly ICustomerManager _customerManager;

        public FeedbackManager(IRepository repository, IOrderManager ordermanager, ICustomerManager customerManager, IDistributedCacheManager distributedCacheManager, IOptions<CacheOptions> cacheOptions)
            : base(distributedCacheManager, cacheOptions)
        {
            _repository = repository;
            _orderManager = ordermanager;
            _customerManager = customerManager;
        }

        public async Task<Feedback> CreateAsync(long userId, long orderId, Feedback feedback, CancellationToken cancellationToken)
        {
            var customer = await _customerManager.GetCustommerByIdAsync(userId, cancellationToken);
            var order = await _orderManager.GetOrderByIdAsync(orderId, cancellationToken);

            // We first check if the order is valid
            if (order.CustomerSid != customer.Sid)
            {
                throw new ArgumentException(string.Format(OrderErrorMessages.OrderNotOwnedByUser, userId, orderId));
            }

            // Now we check if the order has already been rated and the feedback exists.
            // if the feedback does noot exist, an exception will be thrown.
            Feedback existingFeedback = default;
            try
            {
                existingFeedback = await GetAsync(userId, orderId, cancellationToken);
            }
            catch (ArgumentException)
            {
            }

            if (existingFeedback != null)
            {
                throw new ArgumentException(FeedbackErrorMessages.FeedbackAlreadyExists);
            }

            // if all the filters passed, then we can create the new feedback using the repository
            feedback.CreateTime = DateTime.UtcNow;
            var savedFeedback = await _repository.CreateAsync(feedback, cancellationToken);

            // Update the order
            //order.FeedbackS = savedFeedback;
            order.FeedbackSid = savedFeedback.Sid;
            await _orderManager.UpdateOrderAsync(order, cancellationToken);

            return savedFeedback;
        }

        public async Task DeleteAsync(long orderId, CancellationToken cancellationToken)
        {
            var order = await _orderManager.GetOrderByIdAsync(orderId, cancellationToken);
            Feedback feedback = default;
            if (feedback == null)
            {
                // nothing to delete
                throw new ArgumentException(FeedbackErrorMessages.FeedbackDoesNotExists);
            }

            await _repository.DeleteAsync(feedback, cancellationToken);

            feedback = null;
            await _orderManager.UpdateOrderAsync(order, cancellationToken);
        }

        public async Task<Feedback> GetAsync(long userId, long orderId, CancellationToken cancellationToken)
        {
            var customer = await _customerManager.GetCustommerByIdAsync(userId, cancellationToken);
            var order = await _orderManager.GetOrderByIdAsync(orderId, cancellationToken);

            if (order.CustomerSid != customer.Sid)
            {
                throw new ArgumentException(string.Format(OrderErrorMessages.OrderNotOwnedByUser, userId, orderId));
            }

            var feedback = await _repository.GetAsync<Feedback>(feedback => feedback.Sid == order.FeedbackSid, cancellationToken);

            if (feedback == null)
            {
                throw new ArgumentException(string.Format(OrderErrorMessages.OrderHasNotBeenRated, orderId));
            }

            return feedback;
        }

        public async Task UpdateAsync(long orderId, int rating, string comment, CancellationToken cancellationToken)
        {
            var order = await _orderManager.GetOrderByIdAsync(orderId, cancellationToken);
            Feedback feedback = default;
            if (feedback == null)
            {
                throw new ArgumentException(string.Format(OrderErrorMessages.OrderHasNotBeenRated, orderId));
            }

            if (rating != feedback.Rating)
            {
                feedback.Rating = rating;
            }

            if (comment != feedback.Comment)
            {
                feedback.Comment = comment;
            }

            await _repository.UpdateAsync<Feedback>(feedback, cancellationToken);
        }
    }
}
