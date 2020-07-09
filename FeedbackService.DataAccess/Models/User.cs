namespace FeedbackService.DataAccess.Models
{
    public partial class User : BaseEntity
    {
        public long Sid { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
