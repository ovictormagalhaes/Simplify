using Simplify.ORM.Enumerations;
using Simplify.ORM;
using Simplify.ORM.Attributes;

namespace Simplify.Examples.API.Entities
{
    [Table(NamingConvention.SnakeCase)]
    public partial class Profile : SimplifyEntity
    {
        public int ProfileId { get; set; }
        public string? Name { get; set; }
        public string? BirthedAt { get; set; }
        public string? PhoneNumber { get; set; }

        public virtual User? User { get; set; }
    }
}
