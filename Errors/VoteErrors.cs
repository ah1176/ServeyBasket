namespace Survey_Basket.Errors
{
    public static class VoteErrors
    {
        public static readonly Error DuplicateVote = new("Duplicate Vote", "this user already voted on this poll", StatusCodes.Status409Conflict);

        public static readonly Error InvalidVote = new("Invalid Vote", "this vote is invalid", StatusCodes.Status400BadRequest);
    }
}
