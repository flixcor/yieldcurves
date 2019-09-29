using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.Core;
using Microsoft.AspNetCore.SignalR;

namespace Common.Infrastructure.SignalR
{
    public class SignalRRepositoryDecorator<T> : IReadModelRepository<T> where T : ReadObject
    {
        private readonly IReadModelRepository<T> _decorated;
        private readonly IHubCallerClients _clients;

        public SignalRRepositoryDecorator(IReadModelRepository<T> decorated, IHubCallerClients clients)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            _clients = clients;
        }

        public Task<Maybe<T>> Get(Guid id) => _decorated.Get(id);

        public Task<IEnumerable<T>> GetAll() => _decorated.GetAll();

        public Task<IEnumerable<T>> GetMany(Expression<Func<T, bool>> where) => _decorated.GetMany(where);

        public Task<Maybe<T>> Single(Expression<Func<T, bool>> where) => _decorated.Single(where);

        public Task Insert(T t)
        {
            var repoTask = _decorated.Insert(t);
            var hubTask = _clients.All.SendAsync("insert", t);

            return Task.WhenAll(repoTask, hubTask);
        }

        public Task Update(T t)
        {
            var repoTask = _decorated.Update(t);
            var hubTask = _clients.All.SendAsync("update", t);

            return Task.WhenAll(repoTask, hubTask);
        }
    }
}
