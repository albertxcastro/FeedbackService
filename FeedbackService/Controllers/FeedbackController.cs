using FeedbackService.Attributes;
using FeedbackService.DataAccess.Models;
using FeedbackService.Managers.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackManager _feedbackManager;

        public FeedbackController(IFeedbackManager feedbackManager)
        {
            _feedbackManager = feedbackManager;
        }

        // POST: api/Feedback/5445
        [HttpPost("{orderId}")]
        public async Task<ActionResult> PostAsync(long orderId, [FromBody] Feedback value, CancellationToken cancellationToken)
        {
            try
            {
                var userId = ValidateUserIdInHeader();
                var feedback = await _feedbackManager.CreateAsync(userId, orderId, value, cancellationToken);
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
                feedback = await _feedbackManager.GetAsync(userId, orderId, cancellationToken);
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
                feedbackList = await _feedbackManager.GetLatestAsync(rating, cancellationToken);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

            return Ok(feedbackList);
        }

        // PUT: api/Feedback/5
        [HttpPut("{orderId}")]
        public async Task<ActionResult<Feedback>> PutAsync(long orderId, [FromBody] Feedback value, CancellationToken cancellationToken)
        {
            Feedback updatedFeedback;
            try
            {
                var userId = ValidateUserIdInHeader();
                updatedFeedback = await _feedbackManager.UpdateAsync(userId, orderId, value, cancellationToken);
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
                await _feedbackManager.DeleteAsync(userId, orderId, cancellationToken);
            }
            catch (Exception ex)
            {
                return Content(string.Format("{0}: {1}", ex.Message, ex.InnerException.Message));
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
