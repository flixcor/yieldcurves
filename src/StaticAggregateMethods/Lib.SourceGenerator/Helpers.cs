using System;
using System.Collections.Generic;
using System.Text;
using NJsonSchema;


namespace Lib.SourceGenerator
{
    public static class Helpers
    {
        public static string GetContractsText(IEnumerable<(string, string)> files)
        {
            var setupBuilder = new StringBuilder();
            setupBuilder.AppendLine("        public static void Setup()");
            setupBuilder.AppendLine("        {");

            var builder = new StringBuilder();
            builder.AppendLine("#nullable enable");
            builder.AppendLine("namespace Contracts");
            builder.AppendLine("{");
            builder.AppendLine("    using System.Text.Json;");
            builder.AppendLine();
            builder.AppendLine("    public static class ContractCollection");
            builder.AppendLine("    {");


            foreach (var (className, schemaText) in files)
            {
                GetClassContent(className, schemaText, builder);
                setupBuilder.Append("            Lib.AspNet.Serializer.TryRegister(");
                setupBuilder.Append(className);
                setupBuilder.AppendLine(".FromJson);");
                setupBuilder.Append("            Lib.AspNet.SchemaResolver.TryRegister<");
                setupBuilder.Append(className);
                setupBuilder.Append(">(");
                setupBuilder.Append(className);
                setupBuilder.AppendLine(".Schema);");
            }
            setupBuilder.AppendLine("        }");
            builder.Append(setupBuilder);
            builder.AppendLine("    }");
            builder.AppendLine("}");
            builder.AppendLine("#nullable disable");

            return builder.ToString();
        }

        public static void GetClassContent(string className, string contract, StringBuilder recordBuilder)
        {
            var schema = JsonSchema.FromJsonAsync(contract).GetAwaiter().GetResult();

            var extensionBuilder = new StringBuilder("            public static ");
            extensionBuilder.Append(className);
            extensionBuilder.Append(" FromJson(JsonElement element) => new ");
            extensionBuilder.Append(className);
            extensionBuilder.Append("(");

            recordBuilder.Append("        public record ");
            recordBuilder.Append(className);
            recordBuilder.Append("(");

            var counter = 0;

            foreach (var keyValue in schema.Properties)
            {
                var key = keyValue.Key;
                var value = keyValue.Value;

                if (counter != 0)
                {
                    recordBuilder.Append(", ");
                    extensionBuilder.Append(", ");
                }

                var pName = "prop" + counter;


                var typePrefix = value.GetTypePrefix();

                if (!value.IsRequired || value.IsNullable(SchemaType.JsonSchema))
                {
                    typePrefix += "?";
                }

                recordBuilder.Append(typePrefix);


                recordBuilder.Append(" ");
                recordBuilder.Append(key.ToPascalCase());
                value.WriteProperty(extensionBuilder, key, typePrefix, counter);

                counter++;
            }
            extensionBuilder.AppendLine(");");

            recordBuilder.AppendLine(")");
            recordBuilder.AppendLine("        {");
            recordBuilder.Append(extensionBuilder);
            recordBuilder.Append("public const string Schema = @\"");
            recordBuilder.Append(contract.Replace("\"", "\"\""));
            recordBuilder.AppendLine("\";");
            recordBuilder.AppendLine("        }");
        }

        public static bool EqualsIgnoreCase(this string left, string right) => left.Equals(right, StringComparison.OrdinalIgnoreCase);

        public static void WriteProperty(this JsonSchemaProperty property, StringBuilder stringBuilder, string propertyName, string propertyType, int counter)
        {
            if (property.IsRequired && !property.IsNullable(SchemaType.JsonSchema))
            {
                stringBuilder.Append("element.GetProperty(\"");
                stringBuilder.Append(propertyName);
                stringBuilder.Append("\").");
                stringBuilder.Append(property.GetPropertyTypeResolver());
                stringBuilder.Append("()");
                return;
            }

            stringBuilder.Append("!element.TryGetProperty(\"");
            stringBuilder.Append(propertyName);
            stringBuilder.Append("\", out ");
            if (counter == 0)
            {
                stringBuilder.Append("var ");
            }

            stringBuilder.Append("prop)? (");
            stringBuilder.Append(propertyType);
            stringBuilder.Append(")null : prop.");
            stringBuilder.Append(property.GetPropertyTypeResolver());
            stringBuilder.Append("()");
        }

        public static string GetPropertyTypeResolver(this JsonSchemaProperty property) => property.Type switch
        {
            JsonObjectType.Boolean => "GetBoolean",
            JsonObjectType.Integer => "GetInt32",
            JsonObjectType.Number => "GetDouble",
            JsonObjectType.String => "GetString",
            _ => throw new Exception(),
        };

        public static string GetTypePrefix(this JsonSchemaProperty property) => property.Type switch
        {
            JsonObjectType.Boolean => "bool",
            JsonObjectType.Integer => "int",
            JsonObjectType.Number => "double",
            JsonObjectType.String => "string",
            _ => throw new Exception(),
        };

        public static string ToPascalCase(this string camelCase)
        {
            if (string.IsNullOrWhiteSpace(camelCase))
            {
                return camelCase;
            }

            var first = camelCase[0].ToString().ToUpperInvariant();
            var rest = camelCase.Substring(1);

            return first + rest;
        }
    }
}
