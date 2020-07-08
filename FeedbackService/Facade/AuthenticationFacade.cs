using CachingManager.Managers;
using FeedbackService.DataAccess.Models;
using FeedbackService.Facade.Interfaces;
using FeedbackService.Managers.Interfaces;
using FeedbackService.Options;
using FeedbackService.StringConstants.ErrorMessages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Facade
{
    public class AuthenticationFacade : BaseFacade, IAuthenticationFacade
    {
        private readonly IRepository _repository;

        public AuthenticationFacade(IRepository repository, IDistributedCacheManager distributedCacheManager, IOptions<CacheOptions> cacheOptions)
            : base(distributedCacheManager, cacheOptions)
        {
            _repository = repository;
        }

        public async Task<bool> Authenticate(string username, string password, CancellationToken cancellationToken)
        {
            var typeName = nameof(User);
            var entityHash = string.Concat(username, "_", password).GetHashCode();
            var cachedUsers = await GetFromCacheAsync<User>(typeName, entityHash.ToString(), UserErrorMessages.UnableToRetrieveUser, cancellationToken);

            //If the user is already in cache, we just return true
            if (cachedUsers != null && cachedUsers.Select(user => user.Username == username && user.Password == password).Any())
            {
                return true;
            }

            //otherwise, we check in the database and if the user is valid, we cache it
            var user = await _repository.GetAsync<User>(user => user.Username == username && user.Password == password, cancellationToken);
            if (user != null)
            {
                var userList = new List<User> { user };
                await SetCacheAsync(userList, typeName, entityHash.ToString(), cancellationToken);
                return true;
            }

            return false;
        }
    }
}
