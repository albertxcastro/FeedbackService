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

        [HttpPost("{orderId}")]
        public async Task<ActionResult> PostAsync(long orderId, [FromBody] Feedback value, CancellationToken cancellationToken)
        {
            try
            {
                var userId = ValidateUserIdInHeader();
                var feedback = await _orderFeedbackFacade.CreateAsync(userId, orderId, value, cancellationToken);
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<Feedback>> GetAsync(long orderId, CancellationToken cancellationToken)
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

        [HttpGet("GetLatest/{rating?}")]
        public async Task<ActionResult<Feedback>> GetLatestAsync(int? rating, CancellationToken cancellationToken)
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

        [HttpPut("{orderId}")]
        public async Task<ActionResult<Feedback>> PutAsync(long orderId, [FromBody] Feedback value, CancellationToken cancellationToken)
        {
            Feedback updatedFeedback;
            try
            {
                var userId = ValidateUserIdInHeader();
                updatedFeedback = await _orderFeedbackFacade.UpdateAsync(userId, orderId, value, cancellationToken);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

            return Ok(updatedFeedback);
        }

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
                return Content(string.Format("{0}: {1}", ex.Message, ex.InnerException?.Message));
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
