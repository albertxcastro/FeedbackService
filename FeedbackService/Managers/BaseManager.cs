using FeedbackService.DataAccess.Context;
using FeedbackService.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

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
