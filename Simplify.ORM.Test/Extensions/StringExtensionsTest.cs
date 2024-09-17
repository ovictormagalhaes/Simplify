using Xunit;
using Simplify.ORM.Extensions;

namespace Simplify.ORM.Test.Extensions
{
    public class StringExtensionsTest
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("hello", "hello")]
        [InlineData("Hello", "hello")]
        [InlineData("HELLO", "hello")]
        [InlineData("Hello World", "helloWorld")]
        [InlineData("HelloWorld", "helloWorld")]
        [InlineData("hello_world", "helloWorld")]
        [InlineData("hello-world", "helloWorld")]
        [InlineData("Hello_World_Test", "helloWorldTest")]
        [InlineData("SINGLEWORD", "singleword")]
        [InlineData(" multiple   spaces ", "multipleSpaces")]
        [InlineData("a_b_c", "aBC")]
        [InlineData("a-b-c", "aBC")]
        [InlineData("a b c", "aBC")]
        [InlineData("ABC_DEF", "abcDef")]
        [InlineData("abc-DEF", "abcDef")]
        [InlineData("abc DEF", "abcDef")]
        [InlineData("Abc Def", "abcDef")]
        [InlineData("ABC DEF GHI", "abcDefGhi")]
        [InlineData("SimplifyEntityId", "simplifyEntityId")]
        public void ToCamelCase_ShouldConvertCorrectly(string input, string expected)
        {
            // Act
            var result = input.ToCamelCase();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("hello", "Hello")]
        [InlineData("Hello", "Hello")]
        [InlineData("HELLO", "HELLO")]
        [InlineData("Hello World", "HelloWorld")]
        [InlineData("HelloWorld", "HelloWorld")]
        [InlineData("hello_world", "HelloWorld")]
        [InlineData("hello-world", "HelloWorld")]
        [InlineData("Hello_World_Test", "HelloWorldTest")]
        [InlineData("SINGLEWORD", "SINGLEWORD")]
        [InlineData(" multiple   spaces ", "MultipleSpaces")]
        [InlineData("alreadyCamelCase", "AlreadyCamelCase")]
        [InlineData("singleWord", "SingleWord")]
        [InlineData("SingleWord", "SingleWord")]
        [InlineData("a_b_c", "ABC")]
        [InlineData("a-b-c", "ABC")]
        [InlineData("a b c", "ABC")]
        [InlineData("ABC_DEF", "ABCDEF")]
        [InlineData("abc-DEF", "AbcDEF")]
        [InlineData("abc DEF", "AbcDEF")]
        [InlineData("Abc Def", "AbcDef")]
        [InlineData("ABC DEF GHI", "ABCDEFGHI")]
        [InlineData("SimplifyEntityId", "SimplifyEntityId")]
        public void ToPascalCase_ShouldConvertCorrectly(string input, string expected)
        {
            // Act
            var result = input.ToPascalCase();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("hello", "hello")]
        [InlineData("Hello", "hello")]
        [InlineData("HELLO", "hello")]
        [InlineData("Hello World", "hello_world")]
        [InlineData("HelloWorld", "hello_world")]
        [InlineData("hello_world", "hello_world")]
        [InlineData("hello-world", "hello_world")]
        [InlineData("Hello_World_Test", "hello_world_test")]
        [InlineData("SINGLEWORD", "singleword")]
        [InlineData(" multiple   spaces ", "multiple_spaces")]
        [InlineData("alreadyCamelCase", "already_camel_case")]
        [InlineData("singleWord", "single_word")]
        [InlineData("SingleWord", "single_word")]
        [InlineData("a_b_c", "a_b_c")]
        [InlineData("a-b-c", "a_b_c")]
        [InlineData("a b c", "a_b_c")]
        [InlineData("ABC_DEF", "abc_def")]
        [InlineData("abc-DEF", "abc_def")]
        [InlineData("abc DEF", "abc_def")]
        [InlineData("Abc Def", "abc_def")]
        [InlineData("ABC DEF GHI", "abc_def_ghi")]
        [InlineData("SimplifyEntityId", "simplify_entity_id")]
        public void ToSnakeCase_ShouldConvertCorrectly(string input, string expected)
        {
            // Act
            var result = input.ToSnakeCase();

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
