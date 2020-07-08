using FeedbackService.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Facade.Interfaces
{
    public interface IOrderFacade
    {
        Task<Order> GetOrderByIdAsync(long orderId, CancellationToken cancellationToken);
        //Task UpdateOrderAsync(Order order, CancellationToken cancellationToken);
    }
}
