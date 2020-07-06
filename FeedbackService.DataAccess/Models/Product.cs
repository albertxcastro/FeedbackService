using System;
using System.Collections.Generic;

namespace FeedbackService.DataAccess.Models
{
    public partial class Product : BaseEntity
    {
        //public Product()
        //{
        //    OrderToProduct = new HashSet<OrderToProduct>();
        //}

        public string Name { get; set; }
        public float Price { get; set; }

        //public virtual ICollection<OrderToProduct> OrderToProduct { get; set; }
    }
}
