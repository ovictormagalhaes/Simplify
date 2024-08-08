using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xunit;
using SampleSourceGenerator;

namespace Simplify.ORM.Test
{
    public class ColumnValuesSourceGeneratorTests
    {
        [Fact]
        public void Test()
        {
            var a = ClassNames.Test;

            var expected = "Hello from Victor";

            Assert.Equal(expected, a);
        }
        /*

        [Fact]
        public void UserMock_GeneratesCorrectCode()
        {
            var user = new UserMock {
                UserId = 1,
                Username = "Victor",
                Password = "secret",
                CreatedAt = DateTime.Now
            };

            var getColumnValues = user.GetColumnValues();

            var expected = new Dictionary<string, object>()
            {
                { nameof(UserMock.UserId), user.UserId },
                { nameof(UserMock.Username), user.Username },
                { nameof(UserMock.Password), user.Password },
                { nameof(UserMock.CreatedAt), user.CreatedAt }
            };
            Assert.Equal(expected, getColumnValues);
        }

        private string GetSourceCode()
        {
            var sb = new StringBuilder();

            sb.Append(@"
                using System;
                using System.Collections.Generic;
            ");

            sb.Append(@"
                namespace Simplify.ORM
                {
                    public interface ISimplifyEntity
                    {
                        Dictionary<string, object> GetColumnValues();
                    }

                    public abstract partial class SimplifyEntity : ISimplifyEntity
                    {
                        public virtual Dictionary<string, object> GetColumnValues() => throw new NotImplementedException();
                    }
                }
            ");

            sb.Append(@"
                namespace Simplify.ORM
                {
                    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
                    public sealed class GenerateGetColumnValuesAttribute : Attribute
                    {
                        public GenerateGetColumnValuesAttribute()
                        {

                        }
                    }
                }
            ");

            sb.Append(@"
                namespace Simplify.ORM.Test.Mocks
                {
                    [GenerateGetColumnValues]
                    public partial class PermissionMock : SimplifyEntity
                    {
                        public int PermissionId { get; set; }
                        public int Description { get; set; }
                        public DateTime CreatedAt { get; set; }
                    }
                }
            ");

            sb.Append(@"
                namespace Simplify.ORM.Test.Mocks
                {
                    [GenerateGetColumnValues]
                    public partial class UserMock : SimplifyEntity
                    {
                        public int UserId { get; set; }
                        public string? Username { get; set; }
                        public string? Password { get; set; }
                        public DateTime CreatedAt { get; set; }

                        public virtual List<UserPermissionMock>? UserPermission { get; set; }
                    }
                }
            ");

            sb.Append(@"
                namespace Simplify.ORM.Test.Mocks
                {
                    [GenerateGetColumnValues]
                    public partial class UserPermissionMock : SimplifyEntity
                    {
                        public int UserPermissionId { get; set; }
                        public int UserId { get; set; }
                        public int PermissionId { get; set; }

                        public virtual UserMock? User { get; set; }
                        public virtual PermissionMock? Permission { get; set; }
                    }
                }
            ");


            return sb.ToString();
        }

        public static string GetSource2()
        {
            var projectDirectory = "C:\\Users\\ovict\\source\\repos\\Simplify.ORM";

            if (!Directory.Exists(projectDirectory))
            {
                throw new DirectoryNotFoundException($"O diretório '{projectDirectory}' não foi encontrado.");
            }
            //O diretório 'C:\Users\ovict\source\repos\Simplify.ORM\Simplify.ORM.Test\bin\Debug\net8.0\Simplify.ORM' não foi encontrado.'
            var sb = new StringBuilder();
            var csFiles = Directory.GetFiles(projectDirectory, "*.cs", SearchOption.AllDirectories);

            foreach (var file in csFiles)
            {
                var sourceCode = File.ReadAllText(file);
                sb.AppendLine(sourceCode);
                sb.AppendLine(); // Adiciona uma linha em branco entre os arquivos
            }

            return sb.ToString();
        }


        /*
        [Fact]
        public void GenerateGetColumnValues_GeneratesCorrectCode()
        {
            var sourceCode = GetSource2();

            var syntaxTree = CSharpSyntaxTree.ParseText(SourceText.From(sourceCode, Encoding.UTF8));
            var compilation = CSharpCompilation.Create("TestCompilation",
                new[] { syntaxTree },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var generator = new ColumnValuesSourceGenerator();
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

            var diagnostics2 = outputCompilation.GetDiagnostics();

            // Verificar se há algum erro
            Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));

            // Verificar se o código gerado foi adicionado à compilação
            var generatedSyntaxTrees = outputCompilation.SyntaxTrees
                .Where(st => st.FilePath != null && st.FilePath.EndsWith(".g.cs"))
                .ToList();

            foreach (var tree in generatedSyntaxTrees)
            {
                System.Console.WriteLine("Generated syntax tree:");
                System.Console.WriteLine(tree.ToString());
            }

            Assert.NotEmpty(generatedSyntaxTrees);
        }*/
    }
}
