using System;

namespace Simplify.ORM.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class GenerateGetColumnValuesAttribute : Attribute
    {
        public GenerateGetColumnValuesAttribute()
        {

        }
    }
}
