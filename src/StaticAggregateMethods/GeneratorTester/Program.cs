using System;
using System.Text;
using Lib.SourceGenerator;

namespace GeneratorTester
{
    static class Program
    {
        static void Main()
        {
            var builder = new StringBuilder();
            Helpers.GetClassContent("MyClass", schema, builder);
            var str = builder.ToString();
            Console.WriteLine(str);
            Console.WriteLine("press any key");
            Console.ReadKey();
        }

        const string schema = @"
{
""$id"": ""https://example.com/person.schema.json"",
  ""$schema"": ""http://json-schema.org/draft-07/schema#"",
  ""title"": ""Person"",
  ""type"": ""object"",
  ""properties"": {
    ""firstName"": {
      ""type"": ""string"",
      ""description"": ""The person's first name.""
    },
    ""lastName"": {
      ""type"": ""string"",
      ""description"": ""The person's last name.""
    },
    ""age"": {
      ""description"": ""Age in years which must be equal to or greater than zero."",
      ""type"": ""integer"",
      ""minimum"": 0
    }
  }
}
";
    }
}
