using System;
using System.Collections.Generic;

namespace FeedbackService.DataAccess.Models
{
    public partial class Customer : BaseEntity
    {
        //public Customer()
        //{
        //    Order = new HashSet<Order>();
        //}

        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        //public virtual ICollection<Order> Order { get; set; }
    }
}
