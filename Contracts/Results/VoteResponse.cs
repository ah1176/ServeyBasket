namespace Survey_Basket.Contracts.Results
{
    public record VoteResponse(
        string VoterName,
        DateTime Date,
        IEnumerable<QuestionAnswerResponse> Answers
    );

}
