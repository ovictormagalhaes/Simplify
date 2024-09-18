using Simplify.ORM.Attributes;
using Simplify.ORM.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplify.ORM.Test.Entities
{
    [Table(NamingConvention.PascalCase)]
    public partial class UserMockEntity : SimplifyEntity
    {
        public int UserMockId { get; set; }
    }

    [Table(NamingConvention.PascalCase)]
    public partial class MockEntity : SimplifyEntity
    {
        public int MockId { get; set; }
    }

    public partial class ExampleWithoutAttribute : SimplifyEntity
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public DateTime CreatedAt { get; set; }

        public const int SomeConst = 1;
        public virtual List<string>? Permissions { get; set; }
    }

    [Table(NamingConvention.PascalCase)]
    public partial class ExampleTablePascalCase : SimplifyEntity
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    [Table(NamingConvention.CamelCase)]
    public partial class ExampleTableCamelCase : SimplifyEntity
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    [Table(NamingConvention.SnakeCase)]
    public partial class ExampleTableSnakeCase : SimplifyEntity
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    [Table("Custom")]
    public partial class UserCustomMethods : SimplifyEntity
    {
        public override List<SimplifyEntityProperty> GetProperties()
        {
            return [
                new ("Custom", "Custom", "Value")
            ];
        }
    }

    [Table(Name = "Custom2", ColumnsNamingConvention = NamingConvention.PascalCase)]
    public partial class UserCustomMethods2 : SimplifyEntity
    {
        public override List<SimplifyEntityProperty> GetProperties()
        {
            return [new("Custom2", "Custom2", "Value")];
        }
    }

    public partial class MockRelatedEntity : SimplifyEntity
    {
        public int MockRelatedEntityId { get; set; }
        public int RelatedEntityId { get; set; }
        public virtual RelatedEntity RelatedEntity { get; set; }
    }

    public partial class RelatedEntity : SimplifyEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
