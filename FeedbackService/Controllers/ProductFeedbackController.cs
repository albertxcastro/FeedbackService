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
    /// <summary>
    /// Controller responsible for POST/GET/PUT/DELETE comments to existing grocery orders.
    /// Basic Authentication is needed to access the methods.
    /// </summary>
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

        /// <summary>
        /// Asynchronously gets feedback of an item that matches the given productId.
        /// The item must be part of the order that matches the given orderId.
        /// </summary>
        /// <param name="orderId">The order unique id, which is a numeric value.</param>
        /// <param name="productId">The product unique id. The product must be contained in the order.</param>
        /// <param name="cancellationToken">Used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A product's feedback object that matches the productId and that is contained in the order.</returns>
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

        /// <summary>
        /// Asynchronously posts feedback on an item of an existing order. The prouct must haven't been rated yet.
        /// </summary>
        /// <param name="orderId">The order unique id, which is a numeric value.</param>
        /// <param name="productId">The product unique id. The product must be contained in the order.</param>
        /// <param name="newFeedback">The new feedback. It must contain the comment and rating value for the order.</param>
        /// <param name="cancellationToken">Used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The newly created feedback object.</returns>
        [HttpPost("{orderId}/{productId}")]
        public async Task<ActionResult> PostAsync(long orderId, long productId, [FromBody] Feedback newFeedback, CancellationToken cancellationToken)
        {
            try
            {
                var userId = ValidateUserIdInHeader();
                var feedback = await _productFeedbackFacade.CreateAsync(userId, orderId, productId, newFeedback, cancellationToken);
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        /// <summary>
        /// Asynchronously updates a product's feedback that matches the productId and orderId.
        /// </summary>
        /// <param name="orderId">The order unique id, which is a numeric value.</param>
        /// <param name="productId">The product unique id. The product must be contained in the order.</param>
        /// <param name="newFeedback">The new feedback. It must contain the comment and rating value for the order.</param>
        /// <param name="cancellationToken">Used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The newly updated feedback object.</returns>
        [HttpPut("{orderId}/{productId}")]
        public async Task<ActionResult> PutAsync(long orderId, long productId, [FromBody] Feedback newFeedback, CancellationToken cancellationToken)
        {
            try
            {
                var userId = ValidateUserIdInHeader();
                var feedback = await _productFeedbackFacade.UpdateAsync(userId, orderId, productId, newFeedback, cancellationToken);
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        /// <summary>
        /// Asynchronously deletes a product's feedback for an order that matches the given productId and orderId.
        /// </summary>
        /// <param name="orderId">The order unique id, which is a numeric value.</param>
        /// <param name="productId">The product unique id. The product must be contained in the order.</param>
        /// <param name="cancellationToken">Used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A string notifying that the feedback has been correctly deleted.</returns>
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
                return Content(ex.Message);
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
