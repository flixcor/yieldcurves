using System;
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
            Type type = ctor.DeclaringType;
            ParameterInfo[] paramsInfo = ctor.GetParameters();

            //create a single param of type object[]
            ParameterExpression param =
                Expression.Parameter(typeof(object[]), "args");

            Expression[] argsExp =
                new Expression[paramsInfo.Length];

            //pick each arg from the params array 
            //and create a typed expression of them
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;

                Expression paramAccessorExp =
                    Expression.ArrayIndex(param, index);

                Expression paramCastExp =
                    Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created
            NewExpression newExp = Expression.New(ctor, argsExp);

            //create a lambda with the New
            //Expression as body and our param object[] as arg
            LambdaExpression lambda =
                Expression.Lambda(typeof(ObjectActivator), newExp, param);

            //compile it
            ObjectActivator compiled = (ObjectActivator)lambda.Compile();
            return compiled;
        }

        public static EventEnvelope Create(string aggregateId, int version, object content)
        {
            var type = content.GetType();
            var constructor = typeof(EventEnvelope<>).MakeGenericType(content.GetType()).GetConstructor(new[] { typeof(string), typeof(int), type });
            return GetActivator(constructor)(aggregateId, version, content) as EventEnvelope;
        }

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
