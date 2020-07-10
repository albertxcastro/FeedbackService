using FeedbackService.Attributes;
using FeedbackService.DataAccess.Models;
using FeedbackService.Facade.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
    public class OrderFeedbackController : ControllerBase
    {
        private readonly IOrderFeedbackFacade _orderFeedbackFacade;

        public OrderFeedbackController(IOrderFeedbackFacade orderFeedbackFacade)
        {
            _orderFeedbackFacade = orderFeedbackFacade;
        }

        /// <summary>
        /// Asynchronously posts feedback on an existing order. The order must haven't been rated yet.
        /// </summary>
        /// <param name="orderId">The order unique id, which is a numeric value.</param>
        /// <param name="newFeedback">The new feedback. It must contain the comment and rating value for the order.</param>
        /// <param name="cancellationToken">Used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The newly created feedback object.</returns>
        [HttpPost("{orderId}")]
        public async Task<ActionResult> PostAsync(long orderId, [FromBody] Feedback newFeedback, CancellationToken cancellationToken)
        {
            try
            {
                var userId = ValidateUserIdInHeader();
                var feedback = await _orderFeedbackFacade.CreateAsync(userId, orderId, newFeedback, cancellationToken);
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        /// <summary>
        /// Asynchronously gets an order by the given orderId.
        /// </summary>
        /// <param name="orderId">The order unique id, which is a numeric value.</param>
        /// <param name="cancellationToken">Used to propagate notifications that the operation should be canceled.</param>
        /// <returns>An order's feedback object that matches the orderId.</returns>
        [HttpGet("{orderId}")]
        public async Task<ActionResult> GetAsync(long orderId, CancellationToken cancellationToken)
        {
            Feedback feedback;
            try
            {
                var userId = ValidateUserIdInHeader();
                feedback = await _orderFeedbackFacade.GetAsync(userId, orderId, cancellationToken);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

            return Ok(feedback);
        }

        /// <summary>
        /// Asynchronously gets the last 20 order's feedback left by users filtered by the given rating numeric value, in descendant order based on its creation time.
        /// If no rating is provided, the last 20 orders' feedback will be retrieved without filtering by rating.
        /// </summary>
        /// <param name="rating">The rating to filter the order's feedback.</param>
        /// <param name="cancellationToken">Used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A list of the last 20 feedback entries left by users, ordered by creation time descendant and filtered by rating.</returns>
        [HttpGet("GetLatest/{rating?}")]
        public async Task<ActionResult> GetLatestAsync(int? rating, CancellationToken cancellationToken)
        {
            List<Feedback> feedbackList;
            try
            {
                feedbackList = await _orderFeedbackFacade.GetLatestAsync(rating, cancellationToken);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

            return Ok(feedbackList);
        }

        /// <summary>
        /// Asynchronously updates an order's feedback by the given orderId.
        /// </summary>
        /// <param name="orderId">The order unique id, which is a numeric value.</param>
        /// <param name="newFeedback">The new feedback. It must contain the comment and rating value for the order.</param>
        /// <param name="cancellationToken">Used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The newly updated feedback object.</returns>
        [HttpPut("{orderId}")]
        public async Task<ActionResult> PutAsync(long orderId, [FromBody] Feedback newFeedback, CancellationToken cancellationToken)
        {
            Feedback updatedFeedback;
            try
            {
                var userId = ValidateUserIdInHeader();
                updatedFeedback = await _orderFeedbackFacade.UpdateAsync(userId, orderId, newFeedback, cancellationToken);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

            return Ok(updatedFeedback);
        }

        /// <summary>
        /// Asynchronously deletes an order's feedback for an order that matches the given orderId.
        /// </summary>
        /// <param name="orderId">The order unique id, which is a numeric value.</param>
        /// <param name="cancellationToken">Used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A string notifying that the feedback has been correctly deleted.</returns>
        [HttpDelete("{orderId}")]
        public async Task<ActionResult> DeleteAsync(long orderId, CancellationToken cancellationToken)
        {
            try
            {
                var userId = ValidateUserIdInHeader();
                await _orderFeedbackFacade.DeleteAsync(userId, orderId, cancellationToken);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

            return Ok("Correctly Deleted");
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
