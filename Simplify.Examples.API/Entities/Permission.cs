using Simplify.ORM;
using Simplify.ORM.Attributes;
using Simplify.ORM.Enumerations;

namespace Simplify.Examples.API.Entities
{
    [Table(NamingConvention.SnakeCase)]
    public partial class Permission : SimplifyEntity
    {
        public int PermissionId { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}