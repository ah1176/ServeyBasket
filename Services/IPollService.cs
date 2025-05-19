namespace Survey_Basket.Services
{
    public interface IPollService
    {
        Task<Result<IEnumerable<PollResponse>>> GetAllAsync(CancellationToken cancellation = default);
        Task<Result<IEnumerable<PollResponse>>> GetCurrentAsync(CancellationToken cancellation = default);

        Task<Result<PollResponse>> GetByIdAsync(int id , CancellationToken cancellation = default);

        Task<Result<PollResponse>> AddAsync(PollRequest request , CancellationToken cancellation = default);


        Task<Result> UpdateAsync(int id, PollRequest request , CancellationToken cancellation = default);


        Task<Result> DeleteAsync(int id, CancellationToken cancellation = default);


        Task<Result> TogglePublishedStatusAsync(int id, CancellationToken cancellation = default);
    }
}
