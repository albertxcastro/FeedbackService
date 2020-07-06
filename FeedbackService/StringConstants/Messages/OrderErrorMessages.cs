using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeedbackService.StringConstants.Messages
{
    public static class OrderErrorMessages
    {
        public const string UnableToRetrieveOrder = "Unnable to retieve order";
        public const string OrderDoesNotExists = "Unnable to retieve order with orderId {0}";
        public const string OrderHasNotBeenRated = "Order with Id {0} has not been rated. There is no feedback to retrieve.";
        public const string OrderNotOwnedByUser = "User {0} does not own an order with Id {1}";
    }
}
