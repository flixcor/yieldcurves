using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Lib.SourceGenerator
{
    [Generator]
    public class ContractGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var files = GetFiles(context);
            var text = Helpers.GetContractsText(files);
            context.AddSource("ContractCollection", SourceText.From(text, Encoding.UTF8));
        }

        private IEnumerable<(string, string)> GetFiles(GeneratorExecutionContext context)
        {
            foreach (var file in context.AdditionalFiles)
            {
                var split = file.Path.Split('.');
                var length = split.Length;
                if (length > 2 && split.Last().EqualsIgnoreCase("json") && split[length - 2].EqualsIgnoreCase("schema"))
                {
                    var className = Path.GetFileNameWithoutExtension(file.Path).Replace(".schema", "");
                    var schemaText = file.GetText()!.ToString();
                    yield return (className, schemaText);
                }
            }
        }
    }
}
