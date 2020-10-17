using System;
using Lib.SourceGenerator;

namespace GeneratorTester
{
    static class Program
    {
        static void Main()
        {
            var text = Helpers.GetContractsText(new[] { ("MyClass", Schema) });
            Console.WriteLine(text);
            Console.WriteLine("press any key");
            Console.ReadKey();
        }

        const string Schema = @"
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
