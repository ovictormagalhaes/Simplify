using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Simplify.ORM.Generator
{
    public class ClassInfo
    {
        public string NamespaceName { get; set; }
        public string ClassName { get; set; }
        public List<PropertyDeclarationSyntax> Properties { get; set; }
        public List<MethodDeclarationSyntax> Methods { get; set; }
        public List<AttributeListSyntax> Attributes { get; set; }

        public bool HasMethod(string methodName)
            => Methods.Any(m => m.Identifier.Text == methodName);

        public bool HasAttribute(string attributeName)
        {
            return Attributes
                .SelectMany(al => al.Attributes)
                .Any(a => a.Name.ToString() == attributeName);
        }

        public object GetAttributeValue(string attributeName, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(attributeName) || string.IsNullOrWhiteSpace(argumentName))
                return null;

            var attribute = Attributes
                .SelectMany(al => al.Attributes)
                .FirstOrDefault(a => a.Name.ToString() == attributeName);

            if (attribute == null)
                return null;

            var arguments = attribute.ArgumentList?.Arguments.ToList();
            if (arguments == null || !arguments.Any())
                return null;

            var namedArgument = arguments
                .FirstOrDefault(arg => arg.NameEquals?.Name.Identifier.Text == argumentName);

            if (namedArgument != null)
                return GetArgumentValue(namedArgument.Expression);

            var positionalArgument = arguments
                .FirstOrDefault(arg => arg.NameEquals == null);

            if (positionalArgument != null)
                return GetArgumentValue(positionalArgument.Expression);

            return null;
        }

        private static object GetArgumentValue(ExpressionSyntax expression)
        {
            return expression switch
            {
                LiteralExpressionSyntax literal => literal.Token.Value,
                MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.Text,
                _ => expression.ToString(),
            };
        }

    }
}
