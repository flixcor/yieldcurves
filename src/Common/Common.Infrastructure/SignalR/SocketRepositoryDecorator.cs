using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.Core;
using static Common.Infrastructure.Controller.HelperMethods;

namespace Common.Infrastructure.SignalR
{
    public class SocketRepositoryDecorator<T> : IReadModelRepository<T> where T : ReadObject
    {
        private readonly IReadModelRepository<T> _decorated;
        private readonly ISocketContext _context;
        private static readonly string s_featureName = GetFeatureName(typeof(T));

        public SocketRepositoryDecorator(IReadModelRepository<T> decorated, ISocketContext context)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            _context = context;
        }

        public Task<T?> Get(NonEmptyGuid id) => _decorated.Get(id);

        public IAsyncEnumerable<T> GetAll() => _decorated.GetAll();

        public IAsyncEnumerable<T> GetMany(Expression<Func<T, bool>> where) => _decorated.GetMany(where);

        public Task<T?> Single(Expression<Func<T, bool>> where) => _decorated.Single(where);

        public Task Insert(T t)
        {
            var repoTask = _decorated.Insert(t);
            var hubTask = _context.SendToGroup(s_featureName, t, false);

            return Task.WhenAll(repoTask, hubTask);
        }

        public Task Update(T t)
        {
            var repoTask = _decorated.Update(t);
            var hubTask = _context.SendToGroup(s_featureName, t, true);

            return Task.WhenAll(repoTask, hubTask);
        }
    }
}
