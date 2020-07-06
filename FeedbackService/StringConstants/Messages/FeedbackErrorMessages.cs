﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeedbackService.StringConstants.Messages
{
    public static class FeedbackErrorMessages
    {
        public const string FeedbackAlreadyExists = "The order you are trying to rate has already been rated. Try modifying its feedback instead.";
        public const string FeedbackDoesNotExists = "The feedback you are trying to delete does not exists.";
    }
}