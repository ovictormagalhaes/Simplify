namespace Simplify.Examples.API.Controllers.Requests
{
    public record GetUserFilterRequest(string? Username, string? Email, int? PageSize = 10, int? PageNumber = 1);
}
