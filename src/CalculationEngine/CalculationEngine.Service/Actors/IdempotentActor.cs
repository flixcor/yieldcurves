using System;
using System.Collections.Generic;
using Akka.Persistence;

namespace CalculationEngine.Service.ActorModel
{
    public abstract class IdempotentActor : ReceivePersistentActor
    {
        protected static readonly Action<dynamic> Ignore = _ => { };
        protected static readonly Func<dynamic, bool> DefaultValidation = _ => true;

        private readonly IDictionary<Guid, int> _eventVersions = new Dictionary<Guid, int>();

        public void IdempotentEvent<T>(Action<T> commandHandler, Action<T> recoveryHandler = null, Func<T, bool> validation = null) where T : class, Common.Core.IEvent
        {
            recoveryHandler ??= Ignore;
            validation ??= DefaultValidation;

            var wrapped = WrapRecoveryHandler(recoveryHandler);

            Command<T>(e =>
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

        private Action<T> WrapRecoveryHandler<T>(Action<T> action) where T : Common.Core.IEvent
        {
            return x =>
            {
                AddVersion(x);
                action(x);
            };
        }

        private int GetVersion<T>(T e) where T : Common.Core.IEvent
        {
            return _eventVersions.TryGetValue(e.AggregateId, out var version) ? version : -1;
        }

        private void AddVersion<T>(T e) where T : Common.Core.IEvent
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
