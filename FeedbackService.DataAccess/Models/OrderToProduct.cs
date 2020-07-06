using System;
using System.Collections.Generic;

namespace FeedbackService.DataAccess.Models
{
    public partial class OrderToProduct : BaseEntity
    {
        public long Ordersid { get; set; }
        public long ProductSid { get; set; }
        public decimal Ammount { get; set; }

        public virtual Order Orders { get; set; }
        public virtual Product ProductS { get; set; }
    }
}
