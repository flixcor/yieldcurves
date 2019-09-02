using System.Linq;
using System.Threading.Tasks;
using Common.Core;

namespace Common.Infrastructure.EfCore
{
    public class EfCoreUnitOfWork : IUnitOfWork
    {
        private readonly GenericDbContext _db;

        public EfCoreUnitOfWork(GenericDbContext db)
        {
            _db = db;
        }

        public Task SaveChanges()
        {
            var changes = _db.ChangeTracker.Entries().ToList();

            return _db.SaveChangesAsync();
        }
    }
}
