using FeedbackService.DataAccess.Context;

namespace FeedbackService.Managers
{
    public class BaseManager
    {
        protected DataContext _dbContext;

        public void SetContext(DataContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
