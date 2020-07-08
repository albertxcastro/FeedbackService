using FeedbackService.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Facade.Interfaces
{
    public interface ICustomerFacade
    {
        Task<Customer> GetCustommerByIdAsync(long userId, CancellationToken cancellationToken);
    }
}
