using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Lib.EventSourcing
{
    public record EventEnvelope
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

        public static EventEnvelope Create(string aggregateId, int version, object content) 
            => (GetActivator(content.GetType())(aggregateId, version, content) as EventEnvelope)!;

        public long Position { get; internal set; }
        public string? AggregateId { get; init; }
        public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
        public int Version { get; init; }
        public virtual object? Content { get; init; }
    }

    public record EventEnvelope<T>: EventEnvelope where T : class
    {
        public EventEnvelope(string aggregateId, int version, T content)
        {
            AggregateId = aggregateId;
            Version = version;
            base.Content = content;
            Content = content;
        }

        
        public new T? Content { get; set; }
    }
}
