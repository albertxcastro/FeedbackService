using CachingManager.Managers;
using FeedbackService.DataAccess.Models;
using FeedbackService.Enums;
using FeedbackService.Facade.Interfaces;
using FeedbackService.Managers;
using FeedbackService.Managers.Interfaces;
using FeedbackService.Options;
using FeedbackService.StringConstants.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Facade
{
    public class ProductFeedbackFacade : BaseFacade, IProductFeedbackFacade
    {
        private readonly IRepository _repository;
        private readonly IOrderFacade _orderFacade;
        private readonly ICustomerFacade _customerFacade;
        private readonly IProductFacade _productFacade;
        private readonly IProductFeedbackManager _productFeedbackManager;

        public ProductFeedbackFacade(IRepository repository, IOrderFacade orderFacade, ICustomerFacade customerFacade, IProductFacade productFacade, IDistributedCacheManager distributedCacheManager, IOptions<CacheOptions> cacheOptions)
            : base(distributedCacheManager, cacheOptions)
        {
            _repository = repository;
            _orderFacade = orderFacade;
            _customerFacade = customerFacade;
            _productFacade = productFacade;
            _productFeedbackManager = _repository.GetManagerInstance<ProductFeedbackManager>();
        }

        public async Task<Feedback> CreateAsync(long userId, long orderId, long productId, Feedback feedback, CancellationToken cancellationToken)
        {
            ValidateOrderAndCustomer(userId, orderId, cancellationToken);
            Feedback existingFeedback = default;
            try
            {
                existingFeedback = await GetAsync(userId, orderId, productId, cancellationToken);
            }
            catch (ArgumentException)
            {
            }

            if (existingFeedback != null)
            {
                throw new ArgumentException(FeedbackErrorMessages.ProductFeedbackAlreadyExists);
            }

            ValidateRating(feedback.Rating);

            // if all the filters passed, then we can create the new feedback using the repository
            feedback.CreateTime = DateTime.UtcNow;
            feedback.CustomerSid = userId;
            feedback.OrderSid = orderId;
            feedback.FeedbackType = (int)FeedbackType.Product;

            var savedFeedback = await _productFeedbackManager.CreateFeedbackAsync(feedback, orderId, productId, cancellationToken);

            // Populate products just to display them
            var product = await _productFacade.GetProductByIdAsync(productId, cancellationToken);
            savedFeedback.Products = new List<Product> { product };

            await SetCacheAsync(feedback, orderId, productId, cancellationToken);

            return savedFeedback;
        }

        public async Task<Feedback> GetAsync(long userId, long orderId,long productId, CancellationToken cancellationToken)
        {
            ValidateOrderAndCustomer(userId, orderId, cancellationToken);
            var feedback = await GetFromCacheAsync(productId, orderId, cancellationToken);

            if (feedback != null)
            {
                return feedback;
            }

            var orderToProduct = await _repository.GetAsync<OrderToProduct>(feedback => feedback.Ordersid == orderId && feedback.ProductSid == productId, cancellationToken);

            if(orderToProduct == null) 
            {
                throw new ArgumentException(string.Format(ProductErrorMessages.UnableToRetrieveOrderProducts, orderId));
            }

            if (orderToProduct.FeedbackSid == null)
            {
                throw new ArgumentException(string.Format(ProductErrorMessages.ProductHasNotBeenRated, orderId));
            }

            feedback = await _repository.GetAsync<Feedback>(feedback => feedback.Sid == orderToProduct.FeedbackSid && feedback.FeedbackType == (int)FeedbackType.Product, cancellationToken);
            var product = await _productFacade.GetProductByIdAsync(productId, cancellationToken);
            feedback.Products = new List<Product> { product };
            await SetCacheAsync(feedback, orderId, productId, cancellationToken);

            return feedback;
        }

        public async Task<Feedback> UpdateAsync(long userId, long orderId, long productId, Feedback newFeedback, CancellationToken cancellationToken)
        {
            ValidateRating(newFeedback.Rating);
            var updatedFeedback = await GetAsync(userId, orderId, productId, cancellationToken);

            if (updatedFeedback.Rating != newFeedback.Rating)
            {
                updatedFeedback.Rating = newFeedback.Rating;
            }

            if (updatedFeedback.Comment != newFeedback.Comment)
            {
                updatedFeedback.Comment = newFeedback.Comment;
            }

            // Populate products just to display them
            var product = await _productFacade.GetProductByIdAsync(productId, cancellationToken);
            updatedFeedback.Products = new List<Product> { product };

            _repository.Update(updatedFeedback);
            await SetCacheAsync(updatedFeedback, orderId, productId, cancellationToken);

            return updatedFeedback;
        }

        public async Task DeleteAsync(long userId, long orderId, long productId, CancellationToken cancellationToken)
        {
            ValidateOrderAndCustomer(userId, orderId, cancellationToken);
            var feedback = await GetFromCacheAsync(productId, orderId, cancellationToken);

            feedback ??= await GetAsync(userId, orderId, productId, cancellationToken);
            if (feedback == null)
            {
                throw new ArgumentException(string.Format(ProductErrorMessages.ProductHasNotBeenRated, productId));
            }

            await _productFeedbackManager.DeleteFeedbackAsync(orderId, productId, cancellationToken);

            //Order and feedback must be removed from cache
            await RemoveFromCacheAsync(nameof(Feedback), string.Concat(orderId, "_", productId), cancellationToken);
        }

        private void ValidateOrderAndCustomer(long userId, long orderId, CancellationToken cancellationToken)
        {
            var customer = _customerFacade.GetCustommerByIdAsync(userId, cancellationToken).Result;
            var order = _orderFacade.GetOrderByIdAsync(orderId, cancellationToken).Result;

            // We first check if the order is valid
            if (order.CustomerSid != customer.Sid)
            {
                throw new ArgumentException(string.Format(OrderErrorMessages.OrderNotOwnedByUser, userId, orderId));
            }
        }

        private async Task<Feedback> GetFromCacheAsync(long productId, long orderId, CancellationToken cancellationToken)
        {
            var feedbackTypeName = nameof(Feedback);
            var cachedFeedback = await GetFromCacheAsync<Feedback>(feedbackTypeName, string.Concat(orderId, "_", productId), FeedbackErrorMessages.UnableToRetrieveFeedbackFromCache, cancellationToken);
            var feedback = cachedFeedback?.Where(feedback => feedback.OrderSid == orderId && feedback.FeedbackType == (int)FeedbackType.Product).FirstOrDefault();
            return feedback;
        }

        private async Task SetCacheAsync(Feedback feedback, long orderId, long productId, CancellationToken cancellationToken)
        {
            var feedbackTypeName = nameof(Feedback);
            var feedbackList = new List<Feedback> { feedback };
            await SetCacheAsync(feedbackList, feedbackTypeName, string.Concat(orderId, "_", productId), cancellationToken);
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
