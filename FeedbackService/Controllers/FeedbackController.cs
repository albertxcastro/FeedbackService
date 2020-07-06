using FeedbackService.Attributes;
using FeedbackService.DataAccess.Models;
using FeedbackService.Managers.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackManager _feedbackManager;

        public FeedbackController(IFeedbackManager feedbackManager)
        {
            _feedbackManager = feedbackManager;
        }

        // POST: api/Feedback/5445
        [HttpPost("{orderId}")]
        [AllowAnonymous]
        public async Task<ActionResult> Post(long orderId, [FromBody] Feedback value, CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetUserIdFromHeader();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var feedback = await _feedbackManager.CreateAsync(Int64.Parse(userId), orderId, value, cancellationToken);

                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        // GET: api/Feedback
        [HttpGet("{orderId}")]
        public async Task<ActionResult<Feedback>> GetAsync(long orderId, CancellationToken cancellationToken)
        {
            Feedback feedback = default;

            try
            {
                var userId = GetUserIdFromHeader();
                if (userId == null)
                {
                    return Unauthorized();
                }

                feedback = await _feedbackManager.GetAsync(Int64.Parse(userId), orderId, cancellationToken);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

            return feedback;
        }

        // GET: api/Feedback/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }   

        // PUT: api/Feedback/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private string GetUserIdFromHeader()
        {
            var header = Request.Headers;
            var userId = header["UserId"].FirstOrDefault();
            return userId;
        }
    }
}
