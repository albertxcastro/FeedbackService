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
    public class ProductManager : BaseManager, IProductManager
    {
        private readonly IRepository _repository;

        public ProductManager(IRepository repository, IDistributedCacheManager distributedCacheManager, IOptions<CacheOptions> cacheOptions)
            : base(distributedCacheManager, cacheOptions)
        {
            _repository = repository;
        }

        public async Task<Product> GetProductByIdAsync(long productId, CancellationToken cancellationToken)
        {
            var typeName = nameof(Product);
            var cachedProducts = await GetFromCacheAsync<Product>(typeName, productId.ToString(), ProductErrorMessages.UnableToRetrieveProduct, cancellationToken);
            Product product = default;

            //If the user is already in cache, we just return true
            if (cachedProducts != null)
            {
                product = cachedProducts.Where(product => product.Sid == productId).FirstOrDefault();
                if (product != null)
                {
                    return product;
                }
            }

            product = await _repository.GetAsync<Product>(product => product.Sid == productId, cancellationToken);

            if (product == null)
            {
                // if the product is null, just throw this exception
                throw new ArgumentException(string.Format(ProductErrorMessages.ProductDoesNotExists, productId));
            }

            var orderList = new List<Product> { product };
            await SetCacheAsync(orderList, typeName, productId.ToString(), cancellationToken);
            return product;
        }

        public async Task<List<Product>> GetProductsByOrderIdAsync(long orderId, CancellationToken cancellationToken)
        {
            var typeName = nameof(OrderToProduct);
            var cachedProductsInOrder = await GetFromCacheAsync<OrderToProduct>(typeName, orderId.ToString(), ProductErrorMessages.UnableToRetrieveProduct, cancellationToken);
            List<Product> products = new List<Product>();
            List<OrderToProduct> orderProducts = new List<OrderToProduct>();

            if (cachedProductsInOrder != null)
            {
                orderProducts = cachedProductsInOrder.Where(productSet => productSet.Ordersid == orderId).ToList();
            }

            if (orderProducts == null || orderProducts.Count == 0)
            {
                orderProducts = await _repository.GetListAsync<OrderToProduct>(item => item.Ordersid == orderId, cancellationToken);
            }

            if (orderProducts == null || orderProducts.Count == 0)
            {
                throw new ArgumentException(string.Format(OrderErrorMessages.OrderDoesNotExists, orderId));
            }

            await SetCacheAsync(orderProducts, typeName, orderId.ToString(), cancellationToken);
            var productSids = orderProducts.Select(item => item.ProductSid).ToList();

            products = await _repository.GetListAsync<Product>(product => productSids.Contains(product.Sid), cancellationToken);

            if (products == null || products.Count == 0)
            {
                throw new ArgumentException(string.Format(ProductErrorMessages.UnableToRetrieveOrderProducts, orderId));
            }

            return products;
        }
    }
}
