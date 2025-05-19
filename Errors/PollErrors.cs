namespace Survey_Basket.Errors
{
    public static class PollErrors
    {
        public static readonly Error PollNotFound = new("Not Found", "Poll With This Id Wasn't Found", StatusCodes.Status404NotFound);
        public static readonly Error PollsNotFound = new("Not Found", "No Items In Polls Table", StatusCodes.Status404NotFound);
        public static readonly Error DuplicatePollTitle = new("Duplicate Title", "Cannot Insert Duplicate Title", StatusCodes.Status409Conflict);
    }
}
