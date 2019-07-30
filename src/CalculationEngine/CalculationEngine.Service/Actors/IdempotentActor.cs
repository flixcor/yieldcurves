using System;
using System.Collections.Generic;
using Akka.Persistence;

namespace CalculationEngine.Service.ActorModel
{
    public abstract class IdempotentActor : ReceivePersistentActor
    {
        protected static readonly Action<dynamic> Ignore = _ => { };

        private readonly IDictionary<Guid, int> _eventVersions = new Dictionary<Guid, int>();

        public void IdempotentEvent<T>(Action<T> commandHandler, Action<T> recoveryHandler = null, Func<T, bool> validation = null) where T : Common.Core.Event
        {
            var r = recoveryHandler ?? Ignore;
            var v = validation ?? (_ => true);
            var wrapped = WrapRecoveryHandler(recoveryHandler);

            Command<T>(e =>
            {
                if (GetVersion(e) < e.Version && v(e))
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

        Action<T> WrapRecoveryHandler<T>(Action<T> action) where T : Common.Core.Event
        {
            return x =>
            {
                AddVersion(x);
                action(x);
            };
        }

        private int GetVersion<T>(T e) where T : Common.Core.Event
        {
            return _eventVersions.TryGetValue(e.Id, out var version) ? version : -1;
        }

        private void AddVersion<T>(T e) where T : Common.Core.Event
        {
            if (!_eventVersions.ContainsKey(e.Id))
            {
                _eventVersions.Add(e.Id, e.Version);
            }
            else
            {
                _eventVersions[e.Id] = e.Version;
            }
        }
    }
}
