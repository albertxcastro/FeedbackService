﻿using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace FeedbackService.UnitTests.API.ControllerTests
{
    public class ControllerTestBase
    {
        protected HttpContext GetHttpContext(Dictionary<string, string> headers)
        {
            var httpContext = new DefaultHttpContext();
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpContext.Request.Headers.Add(header.Key, header.Value);
                }
            }

            return httpContext;
        }
    }
}
