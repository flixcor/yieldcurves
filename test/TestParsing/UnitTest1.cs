using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
            var json = "{ \"maturity\": 5, \"somethingElse\": \"etc\"}";
            var obj = JsonDocument.Parse(json);

            var result = obj.TryParse((double d) => Maturity.TryCreate(d));

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

        [Fact]
        public void Test2()
        {
            var json = "{ \"maturity\": 5, \"somethingElse\": \"etc\"}";
            var obj = JsonDocument.Parse(json);

            var result = obj.TryParse((string s) => new SomethingElse(s));

            if (result is Right<Error, SomethingElse> right)
            {
                var somethingElse = (SomethingElse)right;
                Assert.Equal("etc", somethingElse.Value);
            }
            else
            {
                Assert.True(false);
            }
        }
    }

    public class SomethingElse
    {
        public SomethingElse(string value)
        {
            Value = value;
        }

        public string Value { get; }
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

        public static Either<Error, TOut> TryParse<T1, TOut>(
            this JsonDocument document,
            Func<T1, TOut> exp)
                => document.TryParse<TOut>(exp);



        private static Either<Error, TOut> FromJsonDocument<TOut>(MethodInfo methodInfo, JsonDocument document)
        {
            var methodParams = methodInfo.GetParameters().ToArray();

            var parameters = methodInfo.GetParameters().Select(x => GetParameter(typeof(TOut), x, document)).ToArray();

            var errors = parameters.OfType<Left<Error, dynamic>>().SelectMany(x => x.Value.Messages).ToArray();

            if (errors.Any())
            {
                return new Error(errors);
            }

            var succesParameters = parameters.OfType<Right<Error, dynamic>>().Select(x => x.Value).ToArray();

            object result = null;

            if (methodInfo.IsStatic)
            {
                result = methodInfo.Invoke(null, succesParameters);
            }
            else
            {
                var paramterless = typeof(TOut).GetConstructors().SingleOrDefault(c => c.GetParameters().Count() == 0);

                if (paramterless != null)
                {
                    var target = paramterless.Invoke(null);
                    result = methodInfo.Invoke(target, succesParameters);
                }
                else
                {
                    var matching = typeof(TOut).GetConstructors().SingleOrDefault(c =>
                    {
                        var consParams = c.GetParameters().ToArray();
                        var length = consParams.Length;
                        for (int i = 0; i < length; i++)
                        {
                            var consParam = consParams[i];
                            var methodParam = methodParams[i];

                            if (consParam.ParameterType != methodParam.ParameterType)
                            {
                                return false;
                            }
                        }
                        return true;
                    });

                    if (matching != null)
                    {
                        result = matching.Invoke(succesParameters);
                    }
                }
            }


            if (result is Either<Error, TOut> either)
            {
                return either;
            }

            if (result is TOut tOut)
            {
                return tOut;
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

            if (parameterInfo.ParameterType == typeof(string))
            {
                return (dynamic)jsonParam.GetString();
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
