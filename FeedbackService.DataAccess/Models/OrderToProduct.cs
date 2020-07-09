namespace FeedbackService.DataAccess.Models
{
    public partial class OrderToProduct : BaseEntity
    {
        public long Ordersid { get; set; }
        public long ProductSid { get; set; }
        public decimal Ammount { get; set; }
        public long? FeedbackSid { get; set; }
    }
}
