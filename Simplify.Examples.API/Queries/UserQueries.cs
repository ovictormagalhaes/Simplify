using Simplify.Examples.API.Entities;
using Simplify.ORM;
using Simplify.ORM.Builders;
using Simplify.ORM.Interfaces;

namespace Simplify.Examples.API.Queries
{
    public interface IUserQuery
    {
        Task<IEnumerable<User>> GetAsyncByFilter(string? username, string? email, string? name);
        Task<User> FirstOrDefaultByIdAsync(int id);
    }

    public class UserQueries(ISimplifyQueryBuilder builder, ISimplifyCommand command) : IUserQuery
    {
        private readonly ISimplifyQueryBuilder _builder = builder ?? throw new ArgumentNullException(nameof(_builder));
        private readonly ISimplifyCommand _command = command ?? throw new ArgumentNullException(nameof(command));

        public async Task<User> FirstOrDefaultByIdAsync(int id)
        {

            var user = await _command.FirstOrDefaultAsync<User>(new UserGetById(id));
            
            await _command.HydrateAsync<User, Profile>(user, x => x.ProfileId, x => x.Profile, x => x.ProfileId);
            await _command.HydrateAsync<User, UserPermission>(user, x => x.UserId, x => x.Permissions, x => x.UserId);
            await _command.HydrateAsync<UserPermission, Permission>(user.Permissions!, x => x.PermissionId, x => x.Permission, x => x.PermissionId);

            return user;
        }

        public async Task<IEnumerable<User>> GetAsyncByFilter(string? username, string? email, string? name)
        {
            var tableUser = _builder.TableName<User>()!;
            var tableProfile = _builder.TableName<Profile>()!;

            _builder.SelectAllFields(tableUser)
                .From(tableUser)
                .LeftJoin(tableProfile, _builder.ColumnName<Profile>(nameof(Profile.ProfileId)), tableUser, _builder.ColumnName<User>(nameof(User.ProfileId)))
                .WhereEquals(tableUser, _builder.ColumnName<User>(nameof(User.Username)), username, !string.IsNullOrWhiteSpace(username))
                .WhereEquals(tableUser, _builder.ColumnName<User>(nameof(User.Email)), email, !string.IsNullOrWhiteSpace(email))
                .WhereEquals(tableUser, _builder.ColumnName<Profile>(nameof(Profile.Name)), name, !string.IsNullOrWhiteSpace(name));

            var userGetByFilter = new UserGetByFilter(_builder, username, email, name)!;

            var users = await _command.QueryAsync<User>(userGetByFilter);

            await _command.HydrateAsync<User, Profile>(users, x => x.ProfileId, x => x.Profile, x => x.ProfileId);
            await _command.HydrateAsync<User, UserPermission>(users, x => x.UserId, x => x.Permissions, x => x.UserId);
            await _command.HydrateAsync<UserPermission, Permission>(users.SelectMany(x => x.Permissions!), x => x.PermissionId, x => x.Permission, x => x.PermissionId);
            
            return users;
        }
    }

    public class UserGetByFilter : SimplifyQuery
    {
        public UserGetByFilter(ISimplifyQueryBuilder queryBuilder, string? username, string? email, string? name) : base(queryBuilder)
        {
            var tableUser = TableName<User>()!;
            var tableProfile = TableName<Profile>()!;

            QueryBuilder.SelectAllFields(tableUser)
                .From(tableUser)
                .LeftJoin(tableProfile, ColumnName<Profile>(nameof(Profile.ProfileId)), tableUser, ColumnName<User>(nameof(User.ProfileId)))
                .WhereEquals(tableUser, ColumnName<User>(nameof(User.Username)), username, !string.IsNullOrWhiteSpace(username))
                .WhereEquals(tableUser, ColumnName<User>(nameof(User.Email)), email, !string.IsNullOrWhiteSpace(email))
                .WhereEquals(tableUser, ColumnName<Profile>(nameof(Profile.Name)), name, !string.IsNullOrWhiteSpace(name));
        }
    }

    public class UserGetById : AbstractSimplifyQueryBuilder
    {
        public UserGetById(int id)
        {
            var tableUser = TableName<User>()!;

            SelectAllFields(tableUser)
                .AddFrom(tableUser)
                .WhereEquals(nameof(User), nameof(User.UserId), id);
        }
    }
}
