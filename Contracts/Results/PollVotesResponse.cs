namespace Survey_Basket.Contracts.Results
{
    public record PollVotesResponse(
        string Title,
        IEnumerable<VoteResponse> Votes
    );

}
