using FeedbackService.Attributes;
using FeedbackService.DataAccess.Models;
using FeedbackService.Facade.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Controllers
{
    [Route("api/[controller]")]
    [BasicAuth]
    [ApiController]
    public class ProductFeedbackController : ControllerBase
    {
        private readonly IProductFeedbackFacade _productFeedbackFacade;

        public ProductFeedbackController(IProductFeedbackFacade productFeedbackFacade)
        {
            _productFeedbackFacade = productFeedbackFacade;
        }

        [HttpGet("{orderId}/{productId}")]
        public async Task<ActionResult> GetAsync(long orderId, long productId, CancellationToken cancellationToken)
        {
            try
            {
                var userId = ValidateUserIdInHeader();
                var feedback = await _productFeedbackFacade.GetAsync(userId, orderId, productId, cancellationToken);
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost("{orderId}/{productId}")]
        public async Task<ActionResult> PostAsync(long orderId, long productId, [FromBody] Feedback value, CancellationToken cancellationToken)
        {
            try
            {
                var userId = ValidateUserIdInHeader();
                var feedback = await _productFeedbackFacade.CreateAsync(userId, orderId, productId, value, cancellationToken);
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPut("{orderId}/{productId}")]
        public async Task<ActionResult> PutAsync(long orderId, long productId, [FromBody] Feedback value, CancellationToken cancellationToken)
        {
            try
            {
                var userId = ValidateUserIdInHeader();
                var feedback = await _productFeedbackFacade.UpdateAsync(userId, orderId, productId, value, cancellationToken);
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpDelete("{orderId}/{productId}")]
        public async Task<ActionResult> DeleteAsync(long orderId, long productId, CancellationToken cancellationToken)
        {
            try
            {
                var userId = ValidateUserIdInHeader();
                await _productFeedbackFacade.DeleteAsync(userId, orderId, productId, cancellationToken);
                return Ok("Correctly deleted");
            }
            catch (Exception ex)
            {
                return Content(string.Format("{0}: {1}", ex.Message, ex.InnerException?.Message));
            }
        }

        private long ValidateUserIdInHeader()
        {
            var header = Request.Headers;
            var incomingUserId = header["UserId"].FirstOrDefault();
            if (string.IsNullOrEmpty(incomingUserId))
            {
                throw new InvalidCastException("UserId was not set in header");
            }

            return Int64.Parse(incomingUserId);
        }
    }
}
