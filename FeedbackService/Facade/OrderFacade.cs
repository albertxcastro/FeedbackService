using CachingManager.Managers;
using FeedbackService.DataAccess.Models;
using FeedbackService.Facade.Interfaces;
using FeedbackService.Managers;
using FeedbackService.Managers.Interfaces;
using FeedbackService.Options;
using FeedbackService.StringConstants.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Facade
{
    public class OrderFacade : BaseFacade, IOrderFacade
    {
        private readonly IRepository _repository;

        public OrderFacade(IRepository repository, IDistributedCacheManager distributedCacheManager, IOptions<CacheOptions> cacheOptions)
            : base(distributedCacheManager, cacheOptions)
        {
            _repository = repository;
        }

        public async Task<Order> GetOrderByIdAsync(long orderId, CancellationToken cancellationToken)
        {
            var typeName = nameof(Order);
            var cachedOrders = await GetFromCacheAsync<Order>(typeName, orderId.ToString(), OrderErrorMessages.UnableToRetrieveOrder, cancellationToken);
            Order order = default;

            //If the user is already in cache, we just return true
            if (cachedOrders != null)
            {
                order = cachedOrders.Where(order => order.Sid == orderId).FirstOrDefault();
                if (order != null)
                {
                    return order;
                }
            }

            order = await _repository.GetAsync<Order>(order => order.Sid == orderId, cancellationToken);

            if (order == null)
            {
               // if the order is null, just throw this exception
                throw new ArgumentException(string.Format(OrderErrorMessages.OrderDoesNotExists, orderId));
            }

            var orderList = new List<Order> { order };
            await SetCacheAsync(orderList, typeName, orderId.ToString(), cancellationToken);
            return order;
        }
    }
}   
