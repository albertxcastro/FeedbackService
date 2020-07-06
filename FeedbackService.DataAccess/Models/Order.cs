using System;
using System.Collections.Generic;

namespace FeedbackService.DataAccess.Models
{
    public partial class Order : BaseEntity
    {
        //public Order()
        //{
        //    OrderToProduct = new HashSet<OrderToProduct>();
        //}

        public long? FeedbackSid { get; set; }
        public float TotalPrice { get; set; }
        public DateTime CreateTime { get; set; }
        public long CustomerSid { get; set; }

        //public virtual Feedback FeedbackS { get; set; }
        //public virtual ICollection<OrderToProduct> OrderToProduct { get; set; }
    }
}
