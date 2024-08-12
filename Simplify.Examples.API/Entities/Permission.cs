using Simplify.ORM;
using Simplify.ORM.Attributes;

namespace Simplify.Examples.API.Entities
{
    [Table]
    public partial class Permission : SimplifyEntity
    {
        public int PermissionId { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}