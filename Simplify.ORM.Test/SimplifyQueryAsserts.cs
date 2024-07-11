namespace Simplify.ORM.Test
{
    public static class SimplifyQueryAsserts
    {
        public static void AssertParameter(ISimplifyQuery query, string parameterName, object parameterValue)
        {
            Assert.True(query.GetParameters().ContainsKey(parameterName));
            Assert.Equal(parameterValue, query.GetParameters()[parameterName]);
        }

    }
}
