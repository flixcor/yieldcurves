using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using NJsonSchema;

namespace Lib.SourceGenerator
{
    [Generator]
    public class ContractGenerator : ISourceGenerator
    {
        private string GetFile(GeneratorExecutionContext context)
        {
            var builder = new StringBuilder();
            builder.AppendLine("namespace Contracts");
            builder.AppendLine("{");
            builder.AppendLine("using System.Text.Json;");
            builder.AppendLine("#nullable enable");
            builder.AppendLine();

            foreach (var file in context.AdditionalFiles)
            {
                var split = file.Path.Split('.');
                var length = split.Length;
                if (length > 2 && split.Last().EqualsIgnoreCase("json") && split[length - 2].EqualsIgnoreCase("schema"))
                {
                    var className = Path.GetFileNameWithoutExtension(file.Path).Replace(".schema", "");
                    var schemaText = file.GetText()!.ToString();
                    Helpers.GetClassContent(className, schemaText, builder);
                }
            }
            builder.AppendLine("#nullable disable");
            builder.AppendLine("}");


            return builder.ToString();
        }


        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            context.AddSource("Contracts", SourceText.From(GetFile(context), Encoding.UTF8));
        }
    }
}
