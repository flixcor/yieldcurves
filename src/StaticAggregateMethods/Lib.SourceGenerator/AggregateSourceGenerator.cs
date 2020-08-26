using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using Lib.Aggregates;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CheeseSourceGenerator
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        public void Execute(SourceGeneratorContext context)
        {
            const string Part1 =
@"namespace Lib
{
    using System;
    using System.Collections.Generic;

    public static class AggregateRegistry
    {
        private static readonly Dictionary<Type, IAggregate> s_aggregates = new Dictionary<Type, IAggregate>();

        static AggregateRegistry()
        {
";



            const string Part2 =
@"
        }

        public static IAggregate<T> Resolve<T>() where T: class, new() => s_aggregates[typeof(T)] as IAggregate<T>;
    }
}
";
            if (!(context.SyntaxReceiver is SyntaxReceiver syntaxReceiver))
            {
                return;
            }
            var compilation = context.Compilation;
            var references = compilation.ExternalReferences.Where(x => !x.Display.Contains("Microsoft.NETCore.App.Ref"));
            var assemblies = references.Select(a => Assembly.LoadFile(a.Display));

            var types = assemblies
                .SelectMany(a => a
                    .GetTypes()
                    .Where(x =>
                        x.BaseType != null
                        && x.BaseType.IsGenericType
                        && x.BaseType.GetGenericTypeDefinition() == typeof(Aggregate<>)))
                    .Select(x => new { Aggregate = x.FullName, State = x.BaseType.GetGenericArguments()[0].FullName })
                    .ToList();


            var stringBuilder = new StringBuilder();

            stringBuilder.Append(Part1);

            foreach (var field in syntaxReceiver.CandidateFields)
            {
                var semanticModel = compilation.GetSemanticModel(field.StateType.SyntaxTree);
                var typeInfo = semanticModel.GetTypeInfo(field.StateType).Type as INamedTypeSymbol;
                var nameSpace = typeInfo.ContainingNamespace;
                var nameSpaceName = nameSpace.ToString();
                var fullT = nameSpaceName + "." + typeInfo.Name;

                stringBuilder.AppendLine($"            s_aggregates.Add(typeof({fullT}), new {field.FullName}());");
            }

            foreach (var item in types)
            {
                stringBuilder.AppendLine($"            s_aggregates.Add(typeof({item.State}), new {item.Aggregate}());");
            }

            stringBuilder.Append(Part2);

            var source = stringBuilder.ToString();

            const string DesiredFileName = "AggregateRegistry.cs";

            var sourceText = SourceText.From(source, Encoding.UTF8); // If no encoding specified then SourceText is not debugable

            // Add the "generated" source to the compilation
            context.AddSource(DesiredFileName, sourceText);
        }

        public void Initialize(InitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
            // Advanced usage
        }



        class SyntaxReceiver : ISyntaxReceiver
        {
            public List<RelevantFieldInfo> CandidateFields { get; } = new List<RelevantFieldInfo>();

            /// <summary>
            /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
            /// </summary>
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                // any field with at least one attribute is a candidate for property generation
                if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
                {
                    var baseTypeSyntax = classDeclarationSyntax?.BaseList?.Types.SingleOrDefault(x => x.Type.ToString().Contains("Aggregate"));
                    if (baseTypeSyntax != null)
                    {
                        var genericNameSyntax = baseTypeSyntax.Type as GenericNameSyntax;
                        var identifierNameSyntax = genericNameSyntax.TypeArgumentList.Arguments.First();

                        var info = new RelevantFieldInfo(identifierNameSyntax, classDeclarationSyntax.GetFullName());
                        CandidateFields.Add(info);
                    }
                }
            }
        }

        class RelevantFieldInfo
        {
            public RelevantFieldInfo(TypeSyntax stateType, string fullName)
            {
                StateType = stateType;
                FullName = fullName;
            }

            public TypeSyntax StateType { get; set; }
            public string FullName { get; }
        }
    }

    public static class ClassDeclarationSyntaxExtensions
    {
        public const string NESTED_CLASS_DELIMITER = "+";
        public const string NAMESPACE_CLASS_DELIMITER = ".";

        public static string GetFullName(this ClassDeclarationSyntax source)
        {
            Contract.Requires(null != source);

            var items = new List<string>();
            var parent = source.Parent;
            while (parent.IsKind(SyntaxKind.ClassDeclaration))
            {
                var parentClass = parent as ClassDeclarationSyntax;
                Contract.Assert(null != parentClass);
                items.Add(parentClass.Identifier.Text);

                parent = parent.Parent;
            }

            var nameSpace = parent as NamespaceDeclarationSyntax;
            Contract.Assert(null != nameSpace);
            var sb = new StringBuilder().Append(nameSpace.Name).Append(NAMESPACE_CLASS_DELIMITER);
            items.Reverse();
            items.ForEach(i => { sb.Append(i).Append(NESTED_CLASS_DELIMITER); });
            sb.Append(source.Identifier.Text);

            var result = sb.ToString();
            return result;
        }
    }
}
