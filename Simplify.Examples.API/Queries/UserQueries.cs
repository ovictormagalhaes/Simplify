using Simplify.Examples.API.Entities;
using Simplify.ORM.Builders;
using Simplify.ORM.Interfaces;

namespace Simplify.Examples.API.Queries
{
    public interface IUserQuery
    {
        Task<IEnumerable<User>> GetAsyncByFilter(string username, string email);
        Task<User> FirstOrDefaultByIdAsync(int id);
    }

    public class UserQueries(ISimplifyQuery query) : IUserQuery
    {
        private readonly ISimplifyQuery _query = query;

        public async Task<User> FirstOrDefaultByIdAsync(int id)
        {
            return await _query.FirstOrDefaultAsync<User>(new UserGetById(id));
        }

        public async Task<IEnumerable<User>> GetAsyncByFilter(string username, string email)
        {
            return await _query.QueryAsync<User>(new UserGetByFilter(username, email));
        }
    }

    public class UserGetByFilter : SimplifyQueryBuilder
    {
        public UserGetByFilter(string username, string email)
        {
            SelectAllFields(nameof(User))
                .AddFrom(nameof(User))
                .WhereEquals(nameof(User), nameof(User.Username), username, !string.IsNullOrWhiteSpace(username))
                .WhereEquals(nameof(User), nameof(User.Email), email, !string.IsNullOrWhiteSpace(email));
        }
    }

    public class UserGetById : SimplifyQueryBuilder
    {
        public UserGetById(int id)
        {
            SelectAllFields(nameof(User))
                .AddFrom(nameof(User))
                .WhereEquals(nameof(User), nameof(User.UserId), id);
        }
    }
}
