using System;
using System.Collections.Generic;

namespace FeedbackService.DataAccess.Models
{
    public partial class User : BaseEntity
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
