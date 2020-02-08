using System;
using System.Collections.Generic;
using Akka.Persistence;
using Common.Core;

namespace CalculationEngine.Service.ActorModel
{
    public abstract class IdempotentActor : ReceivePersistentActor
    {
        protected static readonly Action<dynamic> Ignore = _ => { };
        protected static readonly Func<dynamic, bool> DefaultValidation = _ => true;

        private readonly IDictionary<Guid, int> _eventVersions = new Dictionary<Guid, int>();

        public void IdempotentEvent<T>(Action<IEventWrapper<T>> commandHandler, Action<IEventWrapper<T>> recoveryHandler = null, Func<IEventWrapper<T>, bool> validation = null) where T : class, IEvent
        {
            recoveryHandler ??= Ignore;
            validation ??= DefaultValidation;

            var wrapped = WrapRecoveryHandler(recoveryHandler);

            Command<IEventWrapper<T>>(e =>
            {
                if (GetVersion(e) < e.Version && validation(e))
                {
                    Persist(e, ev =>
                    {
                        commandHandler(ev);
                        wrapped(ev);
                    });
                }
            });

            Recover(wrapped);
        }

        private Action<IEventWrapper<T>> WrapRecoveryHandler<T>(Action<IEventWrapper<T>> action) where T : IEvent
        {
            return x =>
            {
                AddVersion(x);
                action(x);
            };
        }

        private int GetVersion<T>(IEventWrapper<T> e) where T : IEvent
        {
            return _eventVersions.TryGetValue(e.AggregateId, out var version) ? version : -1;
        }

        private void AddVersion<T>(IEventWrapper<T> e) where T : IEvent
        {
            if (!_eventVersions.ContainsKey(e.AggregateId))
            {
                _eventVersions.Add(e.AggregateId, e.Version);
            }
            else
            {
                _eventVersions[e.AggregateId] = e.Version;
            }
        }
    }
}
