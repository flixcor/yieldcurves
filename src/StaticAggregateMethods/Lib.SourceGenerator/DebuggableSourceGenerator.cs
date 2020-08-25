using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CheeseSourceGenerator
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        public void Execute(SourceGeneratorContext context)
        {
            if (!(context.SyntaxReceiver is SyntaxReceiver syntaxReceiver))
            {
                return;
            }

            var stringBuilder = new StringBuilder();

            foreach (var item in syntaxReceiver.CandidateFields)
            {
                var baseType = item.BaseList.Types.First().Type.ToString();

                if (baseType.Contains("Aggregate"))
                {
                    var between = baseType.Replace("Aggregate<", "").Replace(">", "");
                    stringBuilder.Append("Map<" + item.Identifier.ToString() + "," + between + ">()");
                }
            }

            string source = @"
namespace GeneratedCheese
{
    public class CheeseChooser
    {
        public string Test => ""{template}"";
        public string BestCheeseForPasta => ""Parmigiano-Reggiano"";
        public string BestCheeseForBakedPotato => ""Mature Cheddar"";
    }
}
".Replace("{template}", stringBuilder.ToString());

            const string desiredFileName = "CheeseChooser.cs";

            SourceText sourceText = SourceText.From(source, Encoding.UTF8); // If no encoding specified then SourceText is not debugable

            // Add the "generated" source to the compilation
            context.AddSource(desiredFileName, sourceText);
        }

        public void Initialize(InitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
            // Advanced usage
        }

        class SyntaxReceiver : ISyntaxReceiver
        {
            public List<ClassDeclarationSyntax> CandidateFields { get; } = new List<ClassDeclarationSyntax>();

            /// <summary>
            /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
            /// </summary>
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                // any field with at least one attribute is a candidate for property generation
                if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax && (classDeclarationSyntax.BaseList?.Types.Any() ?? false))
                {
                    Console.WriteLine(classDeclarationSyntax.ToString());
                    CandidateFields.Add(classDeclarationSyntax);
                }
            }
        }
    }
}
