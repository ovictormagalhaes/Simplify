using Simplify.ORM;
using Simplify.ORM.Attributes;
using Simplify.ORM.Enumerations;

namespace Simplify.Examples.API.Entities
{
    [Table(NamingConvention.SnakeCase)]
    public partial class User : SimplifyEntity
    {
        public int? UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? ProfileId { get; set; }

        public Profile? Profile { get; set; }
        public List<UserPermission>? Permissions { get; set; }
    }
}