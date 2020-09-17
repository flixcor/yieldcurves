using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Lib.EventSourcing
{

    public record EventEnvelope(long Position, string AggregateId, DateTimeOffset Timestamp, int Version, object Content);

    public static class Extensions
    {
        //https://rogerjohansson.blog/2008/02/28/linq-expressions-creating-objects/
        public delegate object ObjectActivator(params object[] args);

        public static ObjectActivator GetActivator(ConstructorInfo ctor)
        {
            var type = ctor.DeclaringType;
            var paramsInfo = ctor.GetParameters();

            //create a single param of type object[]
            var param =
                Expression.Parameter(typeof(object[]), "args");

            var argsExp =
                new Expression[paramsInfo.Length];

            //pick each arg from the params array 
            //and create a typed expression of them
            for (var i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                var paramType = paramsInfo[i].ParameterType;

                var paramAccessorExp =
                    Expression.ArrayIndex(param, index);

                var paramCastExp =
                    Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created
            var newExp = Expression.New(ctor, argsExp);

            //create a lambda with the New
            //Expression as body and our param object[] as arg
            var lambda =
                Expression.Lambda(typeof(ObjectActivator), newExp, param);

            //compile it
            var compiled = (ObjectActivator)lambda.Compile();
            return compiled;
        }

        private static readonly Dictionary<Type, ObjectActivator> s_activators = new Dictionary<Type, ObjectActivator>();

        private static ObjectActivator GetActivator(Type type)
        {
            if (!s_activators.TryGetValue(type, out var activator))
            {
                var constructor = typeof(EventEnvelope<>).MakeGenericType(type).GetConstructor(new[] { typeof(string), typeof(int), type });
                activator = GetActivator(constructor!);
                s_activators[type] = activator;
            }

            return activator;
        }

        public static EventEnvelope CreateEventEnvelope(this object content, string aggregateId, int version)
            => (GetActivator(content.GetType())(aggregateId, version, content) as EventEnvelope)!;
    }

    public record EventEnvelope<T>: EventEnvelope where T : class
    {
        public EventEnvelope(string aggregateId, int version, T content): base(0, aggregateId, DateTimeOffset.UtcNow, version, content)
        {
            Content = content;
        }
        
        public new T Content { get; }
    }
}
