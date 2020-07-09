using CachingManager.Managers;
using FeedbackService.DataAccess.Context;
using FeedbackService.DataAccess.Models;
using FeedbackService.Enums;
using FeedbackService.Facade.Interfaces;
using FeedbackService.Managers.Interfaces;
using FeedbackService.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FeedbackService.UnitTests.Fixture
{
    public class DataFixture
    {
        public Product GetProduct()
        {
            return new Product
            {
                Name = "Coca cola",
                Price = 20,
                Sid = 0
            };
        }

        public Order GetOrder(DateTime createTime)
        {
            return new Order
            {
                CustomerSid = 0,
                FeedbackSid = null,
                Products = null,
                Sid = 0,
                TotalPrice = 0,
                CreateTime = createTime
            };
        }

        public Customer GetCustomer()
        {
            return new Customer
            {
                FirstName = "Alberto",
                LastName = "Castro",
                Password = "password",
                Username = "admin",
                Sid = 0
            };
        }

        public User GetUser()
        {
            return new User
            {
                FirstName = "Alberto",
                LastName = "Castro",
                Password = "password",
                Username = "admin",
                Sid = 0
            };
        }

        public Feedback GetFeedback(DateTime createTime, FeedbackType feedbackType)
        {
            return new Feedback
            {
                Comment = "This is a comment",
                CreateTime = createTime,
                CustomerSid = 0,
                FeedbackType = (int)feedbackType,
                OrderSid = 0,
                Products = null,
                Rating = 5,
                Sid = 0
            };
        }

        public List<Feedback> GetFeedbackList()
        {
            var list = new List<Feedback>();
            for (int rating = 1; rating < 6; rating++)
            {
                for (int i = 0; i < 10 * rating; i++)
                {
                    var feedback = GetFeedback(DateTime.Now, FeedbackType.Order);
                    feedback.Rating = rating;
                    list.Add(feedback);
                }
            }

            return list;
        }

        public CacheOptions GetCacheOptions()
        {
            return new CacheOptions
            {
                ApplicationAlias = "UnitTests",
                Configuration = "Configuration",
                Expiry = new Expiry[]
                {
                    new Expiry
                    {
                        Key = "default",
                        Value = 0
                    }
                }
            };
        }

        public void GetMocks<T>(out Mock<IRepository> mockRepository, out Mock<IDistributedCacheManager> mockCacheManager, out Mock<IOptions<CacheOptions>> mockOptions)
        {
            var cacheOptions = GetCacheOptions();
            mockRepository = new Mock<IRepository>();
            mockCacheManager = new Mock<IDistributedCacheManager>();
            mockCacheManager
                .Setup(cache => cache.SetCacheAsync(It.IsAny<string>(), It.IsAny<List<T>>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>())).Verifiable();

            mockOptions = new Mock<IOptions<CacheOptions>>();
            mockOptions.Setup(option => option.Value).Returns(cacheOptions);
        }

        public void GetMocks(out Mock<IOrderFacade> orderFacadeMock, out Mock<ICustomerFacade> customerFacadeMock, out Mock<IProductFacade> productFacadeMock)
        {
            orderFacadeMock = new Mock<IOrderFacade>();
            customerFacadeMock = new Mock<ICustomerFacade>();
            productFacadeMock = new Mock<IProductFacade>();
        }
    }
}
