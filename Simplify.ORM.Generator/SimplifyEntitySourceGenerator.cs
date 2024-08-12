using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Simplify.ORM.Enumerations;
using Simplify.ORM.Extensions;
using System;
using System.Linq;
using System.Text;

namespace Simplify.ORM.Generator
{
    [Generator]
    public class SimplifyEntitySourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var syntaxProvider = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => node is ClassDeclarationSyntax,
                    transform: static (context, _) => (ClassDeclarationSyntax)context.Node
                );

            var classDeclarations = syntaxProvider
                .Where(static classDeclaration => IsSimplifyEntity(classDeclaration))
                .Select(static (classDeclaration, _) => CreateClassInfo(classDeclaration))
                .Collect();

            context.RegisterSourceOutput(classDeclarations, (context, classInfos) =>
            {
                foreach (var classInfo in classInfos)
                {
                    var source = GenerateSource(classInfo);

                    if (!string.IsNullOrEmpty(source))
                        context.AddSource($"{classInfo.ClassName}{nameof(SimplifyEntitySourceGenerator)}.g.cs", SourceText.From(source, Encoding.UTF8));
                }
            });
        }

        private static bool IsSimplifyEntity(ClassDeclarationSyntax classDeclaration)
        {
            var baseTypes = classDeclaration.BaseList?.Types;
            return baseTypes?.Any(baseType =>
                baseType.Type.ToString() == "SimplifyEntity") ?? false;
        }

        private static ClassInfo CreateClassInfo(ClassDeclarationSyntax classDeclaration)
        {
            var namespaceDeclaration = classDeclaration.Ancestors()
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault();

            var namespaceName = namespaceDeclaration != null
                ? namespaceDeclaration.Name.ToString()
                : "";

            var className = classDeclaration.Identifier.Text;

            var properties = classDeclaration.Members
                .OfType<PropertyDeclarationSyntax>()
                .Where(p => p.Modifiers.All(m => m.Text != "virtual"))
                .ToList();

            var methods = classDeclaration.Members
                .OfType<MethodDeclarationSyntax>()
                .ToList();

            var attributes = classDeclaration.AttributeLists.ToList();

            return new ClassInfo
            {
                NamespaceName = namespaceName,
                ClassName = className,
                Properties = properties,
                Methods = methods,
                Attributes = attributes
            };
        }

        private static string GenerateSource(ClassInfo? classInfo)
        {
            if (classInfo is null)
                return string.Empty;

            var sb = new StringBuilder();

            var attributeTableName = classInfo.GetAttributeValue("Table", "Name")?.ToString() ?? null;
            var attributeTableColumnsNamingConvention = classInfo.GetAttributeValue("Table", "ColumnsNamingConvention")?.ToString() ?? null;

            Enum.TryParse(attributeTableColumnsNamingConvention, ignoreCase: true, out NamingConvention columnNamingConvention);

            sb.Append(
                $"""
                using System;
                using System.Collections.Generic;
                using Simplify.ORM;
                namespace {classInfo.NamespaceName}
                """);
            sb.AppendLine("{");
            sb.AppendLine($"    public partial class {classInfo.ClassName}");
            sb.AppendLine("    {");

            if (!classInfo.HasMethod("GetColumnValues"))
            {
                sb.AppendLine("        public override Dictionary<string, object> GetColumnValues()");
                sb.AppendLine("        {");
                sb.AppendLine("            var columnValues = new Dictionary<string, object>();");


                foreach (var property in classInfo.Properties)
                {
                    var propertyValue = property.Identifier.Text;

                    var propertyName = columnNamingConvention switch {
                        NamingConvention.PascalCase => property.Identifier.Text.ToPascalCase(),
                        NamingConvention.CamelCase => property.Identifier.Text.ToCamelCase(),
                        NamingConvention.SnakeCase => property.Identifier.Text.ToSnakeCase(),
                        _ => property.Identifier.Text
                    };

                    sb.AppendLine($"            columnValues.Add(\"{propertyName}\", {propertyValue});");
                }

                sb.AppendLine("            return columnValues;");
                sb.AppendLine("        }");
            }

            if (!classInfo.HasMethod("GetTableName"))
            {
                if (!string.IsNullOrWhiteSpace(attributeTableName))
                {
                    sb.AppendLine($"        public override string GetTableName() => \"{attributeTableName}\";");
                    sb.AppendLine($"        public new static string TableName => \"{attributeTableName}\";");
                }
                else
                {
                    sb.AppendLine($"        public override string GetTableName() => nameof({classInfo.ClassName});");
                    sb.AppendLine($"        public new string TableName => nameof({classInfo.ClassName});");
                }
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
