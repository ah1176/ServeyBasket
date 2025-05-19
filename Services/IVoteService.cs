﻿using Survey_Basket.Contracts.Votes;

namespace Survey_Basket.Services
{
    public interface IVoteService
    {
        Task<Result> AddAsync(int pollId , string userId , VoteRequest request , CancellationToken cancellation = default);
    }
}
