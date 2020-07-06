using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Managers.Interfaces
{
    public interface IAuthenticationManager
    {
        Task<bool> Authenticate(string username, string password, CancellationToken cancellationToken);
    }
}
