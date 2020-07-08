using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedbackService.DataAccess.Models
{
    public partial class Feedback : BaseEntity
    {
        public long Sid { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreateTime { get; set; }
        public long OrderSid { get; set; }
        public long CustomerSid { get; set; }
        public int FeedbackType { get; set; }

        [NotMapped]
        public List<Product> Products { get; set; }
    }
}
