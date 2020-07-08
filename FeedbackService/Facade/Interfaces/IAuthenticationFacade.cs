using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Facade.Interfaces
{
    public interface IAuthenticationFacade
    {
        Task<bool> Authenticate(string username, string password, CancellationToken cancellationToken);
    }
}
