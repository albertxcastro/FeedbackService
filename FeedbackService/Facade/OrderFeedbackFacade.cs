using CachingManager.Managers;
using FeedbackService.DataAccess.Models;
using FeedbackService.Enums;
using FeedbackService.Facade.Interfaces;
using FeedbackService.Managers;
using FeedbackService.Managers.Interfaces;
using FeedbackService.Options;
using FeedbackService.StringConstants.Messages;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Facade
{
    public class OrderFeedbackFacade : BaseFacade, IOrderFeedbackFacade
    {
        private readonly IRepository _repository;
        private readonly IOrderFacade _orderFacade;
        private readonly ICustomerFacade _customerFacade;
        private readonly IProductFacade _productFacade;
        private readonly IOrderFeedbackManager _orderFeedbackManager;

        public OrderFeedbackFacade(IRepository repository, IOrderFacade orderFacade, ICustomerFacade customerFacade, IProductFacade productFacade, IDistributedCacheManager distributedCacheManager, IOptions<CacheOptions> cacheOptions)
            : base(distributedCacheManager, cacheOptions)
        {
            _repository = repository;
            _orderFacade = orderFacade;
            _customerFacade = customerFacade;
            _productFacade = productFacade;
            _orderFeedbackManager = _repository.GetManagerInstance<OrderFeedbackManager>();
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
                throw new ArgumentException(FeedbackErrorMessages.OrderFeedbackAlreadyExists);
            }

            ValidateRating(feedback.Rating);

            // if all the filters passed, then we can create the new feedback using the repository
            feedback.CreateTime = DateTime.UtcNow;
            feedback.CustomerSid = userId;
            feedback.OrderSid = orderId;
            feedback.FeedbackType = (int)FeedbackType.Order;
            feedback.Products = await _productFacade.GetProductsByOrderIdAsync(orderId, cancellationToken);

            //update order and feedback
            var savedFeedback = await _orderFeedbackManager.CreateFeedbackAsync(order, feedback, cancellationToken);

            // Populate products just to display them
            savedFeedback.Products = await _productFacade.GetProductsByOrderIdAsync(orderId, cancellationToken);

            await SetCacheAsync(feedback, order, cancellationToken);
            await RemoveFromCacheAsync(nameof(Feedback), string.Concat("GetLatest_Rating_", feedback.Rating.ToString()), cancellationToken);

            return savedFeedback;
        }

        public async Task<Feedback> GetAsync(long userId, long orderId, CancellationToken cancellationToken)
        {
            ValidateOrderAndCustomer(userId, orderId, cancellationToken, out Order order);
            var feedback = await GetFromCacheAsync(userId, order, cancellationToken);

            if (feedback != null)
            {
                return feedback;
            }

            feedback = await _repository.GetAsync<Feedback>(feedback => feedback.Sid == order.FeedbackSid && feedback.FeedbackType == (int)FeedbackType.Order, cancellationToken);
            if (feedback == null)
            {
                throw new ArgumentException(string.Format(OrderErrorMessages.OrderHasNotBeenRated, orderId));
            }

            feedback.Products = await _productFacade.GetProductsByOrderIdAsync(orderId, cancellationToken);
            await SetCacheAsync(feedback, order, cancellationToken);

            return feedback;
        }

        public async Task<List<Feedback>> GetLatestAsync(int? rating, CancellationToken cancellationToken)
        {
            var ratingVal = rating.GetValueOrDefault();
            if (ratingVal != 0)
            {
                ValidateRating(ratingVal);
            }

            var typeName = nameof(Feedback);
            var feedbackList = await GetFromCacheAsync<Feedback>(
                typeName, 
                string.Concat("GetLatest_Rating_", ratingVal.ToString()), 
                FeedbackErrorMessages.UnableToRetrieveFeedbackFromCache, 
                cancellationToken);

            if (feedbackList != null)
            {
                return feedbackList;
            }

            if (rating.HasValue)
            {
                feedbackList = (await _repository.GetListAsync<Feedback>(feedback => feedback.Rating == ratingVal, cancellationToken))
                    .OrderByDescending(feedback => feedback.CreateTime)
                    .Take(20).ToList();
            }
            else
            {
                feedbackList = (await _repository.GetAllAsync<Feedback>(cancellationToken))
                    .OrderByDescending(feedback => feedback.CreateTime)
                    .Take(20).ToList();
            }

            foreach (var feedback in feedbackList)
            {
                feedback.Products = await _productFacade.GetProductsByOrderIdAsync(feedback.OrderSid, cancellationToken);
            }

            await SetCacheAsync(
                feedbackList, 
                typeName, 
                string.Concat("GetLatest_Rating_", ratingVal.ToString()), 
                cancellationToken);

            return feedbackList;
        }

        public async Task<Feedback> UpdateAsync(long userId, long orderId, Feedback newFeedback, CancellationToken cancellationToken)
        {
            var updatedFeedback = await GetAsync(userId, orderId, cancellationToken);
            ValidateRating(newFeedback.Rating);

            if (updatedFeedback.Rating != newFeedback.Rating)
            {
                updatedFeedback.Rating = newFeedback.Rating;
            }

            if (updatedFeedback.Comment != newFeedback.Comment)
            {
                updatedFeedback.Comment = newFeedback.Comment;
            }

            // Populate products just to display them
            updatedFeedback.Products = await _productFacade.GetProductsByOrderIdAsync(orderId, cancellationToken);

            _repository.Update(updatedFeedback);

            await SetCacheAsync(updatedFeedback, orderId, cancellationToken);
            await RemoveFromCacheAsync(nameof(Feedback), string.Concat("GetLatest_Rating_", updatedFeedback.Rating.ToString()), cancellationToken);

            return updatedFeedback;
        }

        public async Task DeleteAsync(long userId, long orderId, CancellationToken cancellationToken)
        {
            ValidateOrderAndCustomer(userId, orderId, cancellationToken, out Order order);
            var feedback = await GetFromCacheAsync(userId, order, cancellationToken);

            feedback ??= await _repository.GetAsync<Feedback>(feedback => feedback.Sid == order.FeedbackSid && feedback.FeedbackType == (int)FeedbackType.Order, cancellationToken);
            if (feedback == null)
            {
                throw new ArgumentException(string.Format(OrderErrorMessages.OrderHasNotBeenRated, orderId));
            }

            await _orderFeedbackManager.DeleteFeedbackAsync(order, feedback, cancellationToken);

            //Order and feedback must be removed from cache
            await RemoveFromCacheAsync(nameof(Feedback), orderId.ToString(), cancellationToken);
            await RemoveFromCacheAsync(nameof(Order), orderId.ToString(), cancellationToken);
        }

        private void ValidateOrderAndCustomer(long userId, long orderId, CancellationToken cancellationToken, out Order order)
        {
            var customer = _customerFacade.GetCustommerByIdAsync(userId, cancellationToken).Result;
            order = _orderFacade.GetOrderByIdAsync(orderId, cancellationToken).Result;

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
            var feedback = cachedFeedback?.Where(feedback => feedback.Sid == order.FeedbackSid && feedback.FeedbackType == (int)FeedbackType.Order).FirstOrDefault();
            return feedback;
        }

        private async Task SetCacheAsync(Feedback feedback, Order order, CancellationToken cancellationToken)
        {
            var feedbackList = new List<Feedback> { feedback };
            var orderList = new List<Order> { order };
            cancellationToken.ThrowIfCancellationRequested();
            await SetCacheAsync(feedbackList, nameof(Feedback), order.Sid.ToString(), cancellationToken);
            await SetCacheAsync(orderList, nameof(Order), order.Sid.ToString(), cancellationToken);
        }

        private async Task SetCacheAsync(Feedback feedback, long orderId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var feedbackList = new List<Feedback> { feedback };
            await SetCacheAsync(feedbackList, nameof(Feedback), orderId.ToString(), cancellationToken);
        }

        private void ValidateRating(int rating)
        {
            if (rating < 1 || rating > 5)
            {
                throw new ArgumentException(FeedbackErrorMessages.InvalidRatingValue);
            }
        }
    }
}
