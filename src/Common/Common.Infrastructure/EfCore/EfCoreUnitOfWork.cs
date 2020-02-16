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
            return _db.SaveChangesAsync();
        }
    }
}
