using FeedbackService.DataAccess.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Managers.Interfaces
{
    public interface IProductManager
    {
        Task<Product> GetProductByIdAsync(long productId, CancellationToken cancellationToken);
        Task<List<Product>> GetProductsByOrderIdAsync(long orderId, CancellationToken cancellationToken);
    }
}
