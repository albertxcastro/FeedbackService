using FeedbackService.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Managers.Interfaces
{
    public interface ICustomerManager
    {
        Task<Customer> GetCustommerByIdAsync(long userId, CancellationToken cancellationToken);
    }
}
