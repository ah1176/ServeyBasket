namespace Survey_Basket.Contracts.Users
{
    public record CreateUserRequest(
     
        string Email,
        string FirstName,
        string LastName,
        string Password,
        IList<string> Roles
    );
}
