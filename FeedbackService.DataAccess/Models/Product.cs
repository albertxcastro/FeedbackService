namespace FeedbackService.DataAccess.Models
{
    public partial class Product : BaseEntity
    {
        public long Sid { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
    }
}
