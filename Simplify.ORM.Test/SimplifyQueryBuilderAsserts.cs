using Simplify.ORM.Interfaces;
using Xunit;

namespace Simplify.ORM.Test
{
    public static class SimplifyQueryBuilderAsserts
    {
        public static void AssertParameter(ISimplifyQueryBuilder query, string parameterName, object parameterValue)
        {
            Assert.True(query.GetParameters().ContainsKey(parameterName));
            Assert.Equal(parameterValue, query.GetParameters()[parameterName]);
        }

    }
}
