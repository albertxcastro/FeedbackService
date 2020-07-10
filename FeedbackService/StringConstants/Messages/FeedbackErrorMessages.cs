namespace FeedbackService.StringConstants.Messages
{
    public static class FeedbackErrorMessages
    {
        public const string OrderFeedbackAlreadyExists = "The order you are trying to rate has already been rated. Try modifying its feedback instead.";
        public const string ProductFeedbackAlreadyExists = "The product you are trying to rate has already been rated. Try modifying its feedback instead.";
        public const string FeedbackDoesNotExists = "The feedback you are trying to delete does not exists.";
        public const string UnableToRetrieveFeedbackFromCache = "Unable to retrieve feedback from cache.";
        public const string InvalidRatingValue = "Invalid rating. The rating must be between 1 to 5.";
    }
}
