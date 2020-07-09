using CachingManager.Managers;
using FeedbackService.DataAccess.Models;
using FeedbackService.Facade;
using FeedbackService.Managers.Interfaces;
using FeedbackService.Options;
using FeedbackService.UnitTests.Fixture;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace FeedbackService.UnitTests.FacadeTests
{
    public class AuthenticationFacadeTests : IClassFixture<DataFixture>
    {
        private readonly DataFixture _dataFixture;

        public AuthenticationFacadeTests(DataFixture dataFixture)
        {
            _dataFixture = dataFixture;
        }

        [Fact]
        public void Authenticate_GetFromDb_Test()
        {
            // Arrange
            string testUsername = "admin";
            string testPassword = "password";
            var user = _dataFixture.GetUser();

            List<User> cachedUserList = null;

            _dataFixture.GetMocks<User>(out var mockRepository, out var mockCacheManager, out var mockOptions);

            mockRepository
                .Setup(repo => repo.GetAsync<User>(user => user.Username == testUsername && user.Password == testPassword, CancellationToken.None))
                .ReturnsAsync(user);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<User>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedUserList);

            var authFacade = new AuthenticationFacade(mockRepository.Object, mockCacheManager.Object, mockOptions.Object);

            // Act
            var result = authFacade.Authenticate(testUsername, testPassword, CancellationToken.None).Result;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Authenticate_GetFromCache_Test()
        {
            // Arrange
            string testUsername = "admin";
            string testPassword = "password";
            var user = _dataFixture.GetUser();

            List<User> cachedUserList = new List<User> { user };

            _dataFixture.GetMocks<User>(out var mockRepository, out var mockCacheManager, out var mockOptions);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<User>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedUserList);

            var authFacade = new AuthenticationFacade(mockRepository.Object, mockCacheManager.Object, mockOptions.Object);

            // Act
            var result = authFacade.Authenticate(testUsername, testPassword, CancellationToken.None).Result;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Authenticate_UserDoesNotExistInCacheNorInDb_Test()
        {
            // Arrange
            string testUsername = "admin";
            string testPassword = "password";
            User user = null;

            List<User> cachedUserList = null;

            _dataFixture.GetMocks<User>(out var mockRepository, out var mockCacheManager, out var mockOptions);

            mockRepository
                .Setup(repo => repo.GetAsync<User>(user => user.Username == testUsername && user.Password == testPassword, CancellationToken.None))
                .ReturnsAsync(user);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<User>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedUserList);

            var authFacade = new AuthenticationFacade(mockRepository.Object, mockCacheManager.Object, mockOptions.Object);

            // Act
            var result = authFacade.Authenticate(testUsername, testPassword, CancellationToken.None).Result;

            // Assert
            Assert.False(result);
        }
    }
}
