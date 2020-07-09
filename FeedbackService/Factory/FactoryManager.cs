using FeedbackService.DataAccess.Context;
using FeedbackService.Managers;

namespace FeedbackService.Factory
{
    public class FactoryManager
    {
        public T CreateManager<T>(DataContext context) where T : BaseManager, new()
        {
            var manager = new T();
            manager.SetContext(context);
            return manager;
        }
    }
}
