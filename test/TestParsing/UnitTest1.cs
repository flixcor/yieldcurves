using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Akka.Dispatch.SysMsg;
using Common.Core;
using CurveRecipes.Domain;
using Xunit;

namespace TestParsing
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var json = "{ \"maturity\": 5}";
            var obj = JsonDocument.Parse(json);

            var result = obj.TryParse<double, Maturity>(Maturity.TryCreate);

            if (result is Right<Error, Maturity> right)
            {
                var maturity = (Maturity)right;
                Assert.Equal(5, maturity.Value);
            }
            else
            {
                Assert.True(false);
            }
        }
    }

    public static class Extensions
    {
        public static Either<Error, T> TryParse<T>(this JsonDocument document, Delegate del)
        {
            var methodInfo = del.GetMethodInfo();
            return FromJsonDocument<T>(methodInfo, document);
        }

        public static Either<Error, TOut> TryParse<T1, TOut>(
            this JsonDocument document,
            Func<T1, Either<Error, TOut>> exp) 
                => document.TryParse<TOut>(exp);

        public static Either<Error, TOut> TryParse<T1, T2, TOut>(
            this JsonDocument document,
            Func<T1, T2, Either<Error, TOut>> exp)
                => document.TryParse<TOut>(exp);



        private static Either<Error, TOut> FromJsonDocument<TOut>(MethodInfo methodInfo, JsonDocument document)
        {
            var parameters = methodInfo.GetParameters().Select(x => GetParameter(typeof(TOut), x, document)).ToArray();

            var errors = parameters.OfType<Left<Error, dynamic>>().SelectMany(x => x.Value.Messages).ToArray();

            if (errors.Any())
            {
                return new Error(errors);
            }

            var succesParameters = parameters.OfType<Right<Error, dynamic>>().Select(x => x.Value).ToArray();

            var result = methodInfo.Invoke(null, succesParameters);

            if (result is Either<Error, TOut> either)
            {
                return either;
            }

            return new Error("something went wrong");
        }

        private static Either<Error, dynamic> GetParameter(Type returnType, ParameterInfo parameterInfo, JsonDocument document)
        {
            if (!document.RootElement.TryGetProperty(parameterInfo.Name, out var jsonParam))
            {
                var typeName = returnType.Name;
                var camel = $"{typeName.ToLower()[0]}{typeName.Substring(1)}";

                if (!document.RootElement.TryGetProperty(camel, out jsonParam))
                {
                    return new Error("Not found");
                }
            }

            if (parameterInfo.ParameterType == typeof(double))
            {
                if (jsonParam.TryGetDouble(out var doub))
                {
                    return (dynamic)doub;
                }
                else
                {
                    return new Error($"Expected double, found {jsonParam.ValueKind}");
                }
            }

            return new Error("unsupported type");
        }
    }
}
