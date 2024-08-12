namespace Simplify.Examples.API.Controllers.Requests
{
    public record GetUserFilterRequest(string? Username, string? Email, int? PageSize, int? PageNumber);
}
