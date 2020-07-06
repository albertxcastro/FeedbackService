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
            ValidateOrderAndCustomer(userId, orderId, cancellationToken, out Order order);
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

        public async Task DeleteAsync(long userId, long orderId, CancellationToken cancellationToken)
        {
            ValidateOrderAndCustomer(userId, orderId, cancellationToken, out Order order);
            var feedback = await GetFromCacheAsync(userId, order, cancellationToken);

            feedback ??= await _repository.GetAsync<Feedback>(feedback => feedback.Sid == order.FeedbackSid, cancellationToken);
            if (feedback == null)
            {
                throw new ArgumentException(string.Format(OrderErrorMessages.OrderHasNotBeenRated, orderId));
            }

            order.FeedbackSid = null;
            await _orderManager.UpdateOrderAsync(order, cancellationToken);
            await _repository.DeleteAsync(feedback, cancellationToken);

            //Order and feedback must be removed from cache
            await RemoveFromCacheAsync(nameof(Feedback), orderId.ToString(), cancellationToken);
            await RemoveFromCacheAsync(nameof(Order), orderId.ToString(), cancellationToken);

        }

        public async Task<Feedback> GetAsync(long userId, long orderId, CancellationToken cancellationToken)
        {
            ValidateOrderAndCustomer(userId, orderId, cancellationToken, out Order order);
            var feedback = await GetFromCacheAsync(userId, order, cancellationToken);

            if (feedback != null)
            {
                return feedback;
            }

            feedback = await _repository.GetAsync<Feedback>(feedback => feedback.Sid == order.FeedbackSid, cancellationToken);
            if (feedback == null)
            {
                throw new ArgumentException(string.Format(OrderErrorMessages.OrderHasNotBeenRated, orderId));
            }

            await SetCacheAsync(feedback, order.Sid, cancellationToken);
            return feedback;
        }

        public async Task<Feedback> UpdateAsync(long userId, long orderId, Feedback newFeedback, CancellationToken cancellationToken)
        {
            var updatedFeedback = await GetAsync(userId, orderId, cancellationToken);

            if (updatedFeedback.Rating != newFeedback.Rating)
            {
                updatedFeedback.Rating = newFeedback.Rating;
            }

            if (updatedFeedback.Comment != newFeedback.Comment)
            {
                updatedFeedback.Comment = newFeedback.Comment;
            }

            await _repository.UpdateAsync<Feedback>(updatedFeedback, cancellationToken);
            await SetCacheAsync(updatedFeedback, orderId, cancellationToken);

            return updatedFeedback;
        }

        private void ValidateOrderAndCustomer(long userId, long orderId, CancellationToken cancellationToken, out Order order)
        {
            var customer = _customerManager.GetCustommerByIdAsync(userId, cancellationToken).Result;
            order = _orderManager.GetOrderByIdAsync(orderId, cancellationToken).Result;

            // We first check if the order is valid
            if (order.CustomerSid != customer.Sid)
            {
                throw new ArgumentException(string.Format(OrderErrorMessages.OrderNotOwnedByUser, userId, orderId));
            }
        }

        private async Task<Feedback> GetFromCacheAsync(long userId, Order order, CancellationToken cancellationToken)
        {
            var typeName = nameof(Feedback);
            var cachedFeedback = await GetFromCacheAsync<Feedback>(typeName, order.Sid.ToString(), FeedbackErrorMessages.UnableToRetrieveFeedbackFromCache, cancellationToken);
            var feedback = cachedFeedback?.Where(feedback => feedback.Sid == order.FeedbackSid).FirstOrDefault();
            return feedback;
        }

        private async Task SetCacheAsync(Feedback feedback, long orderId, CancellationToken cancellationToken)
        {
            var typeName = nameof(Feedback);
            var feedbackList = new List<Feedback> { feedback };
            await SetCacheAsync(feedbackList, typeName, orderId.ToString(), cancellationToken);
        }
    }
}
