using System;

namespace FeedbackService.DataAccess.Models
{
    public partial class Order : BaseEntity
    {
        public long Sid { get; set; }
        public long? FeedbackSid { get; set; }
        public float TotalPrice { get; set; }
        public DateTime CreateTime { get; set; }
        public long CustomerSid { get; set; }

        public long[] Products { get; set; }
    } 
}
