using System;
using System.Collections.Generic;

namespace FeedbackService.DataAccess.Models
{
    public partial class Feedback : BaseEntity
    {
        //public Feedback()
        //{
        //    Order = new HashSet<Order>();
        //}

        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreateTime { get; set; }

        //public virtual ICollection<Order> Order { get; set; }
    }
}
