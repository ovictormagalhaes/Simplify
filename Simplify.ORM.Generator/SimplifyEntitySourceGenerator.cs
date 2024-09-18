using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
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

        private static string GenerateSource(ClassInfo classInfo)
        {
            if (classInfo is null)
                return string.Empty;

            var sb = new StringBuilder();

            var attributeTableName = classInfo.GetAttributeValue("Table", "Name")?.ToString() ?? null;
            var attributeTableColumnsNamingConvention = classInfo.GetAttributeValue("Table", "ColumnsNamingConvention")?.ToString() ?? null;

            var tableName = classInfo.ClassName;

            if(attributeTableColumnsNamingConvention != null && Enum.TryParse(attributeTableColumnsNamingConvention, out NamingConvention tableNamingConvention))
            {
                tableName = tableNamingConvention switch
                {
                    NamingConvention.PascalCase => string.Copy(classInfo.ClassName).ToPascalCase(),
                    NamingConvention.CamelCase => string.Copy(classInfo.ClassName).ToCamelCase(),
                    NamingConvention.SnakeCase => string.Copy(classInfo.ClassName).ToSnakeCase(),
                    _ => classInfo.ClassName
                };

            }
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

            if (!classInfo.HasMethod("GetProperties"))
            {
                sb.AppendLine("        public override List<SimplifyEntityProperty> GetProperties()");
                sb.AppendLine("        {");
                sb.AppendLine("            var columnValues = new List<SimplifyEntityProperty>();");

                foreach (var property in classInfo.Properties)
                {
                    var propertyIdentifier = property.Identifier.Text;

                    var columnName = columnNamingConvention switch
                    {
                        NamingConvention.PascalCase => string.Copy(property.Identifier.Text).ToPascalCase(),
                        NamingConvention.CamelCase => string.Copy(property.Identifier.Text).ToCamelCase(),
                        NamingConvention.SnakeCase => string.Copy(property.Identifier.Text).ToSnakeCase(),
                        _ => property.Identifier.Text
                    };

                    sb.AppendLine($"            columnValues.Add(new SimplifyEntityProperty(\"{propertyIdentifier}\", \"{columnName}\", {propertyIdentifier}));");
                }

                sb.AppendLine("            return columnValues;");
                sb.AppendLine("        }");
            }

            if (!classInfo.HasMethod("GetTableName"))
            {
//                if (!string.IsNullOrWhiteSpace(attributeTableName))
//                    sb.AppendLine($"        public override string GetTableName() => \"{attributeTableName}\";");
//                else
                    sb.AppendLine($"        public override string GetTableName() => \"{tableName}\";");
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
