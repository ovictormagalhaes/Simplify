using Simplify.ORM;
using Simplify.ORM.Attributes;

namespace Simplify.Examples.API.Entities
{
    [Table]
    public partial class UserPermission : SimplifyEntity
    {
        public int UserPermissionId { get; set; }
        public int UserId { get; set; }
        public int PermissionId { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual User? User { get; set; }
        public virtual Permission? Permission { get; set; }
    }
}