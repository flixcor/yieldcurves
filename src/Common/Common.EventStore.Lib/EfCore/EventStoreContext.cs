using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Common.EventStore.Lib.EfCore
{
    internal class EventStoreContext : DbContext
    {
        static EventStoreContext()
        {
            NpgsqlConnection.GlobalTypeMapper.UseNodaTime();
        }

        public EventStoreContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PersistedEvent>(e =>
            {
                e.ToTable("Events");
                e.HasIndex(ev => new { ev.AggregateId, ev.Version }).IsUnique();
                e.HasIndex(ev => new { ev.AggregateId });
                e.HasIndex(ev => new { ev.Timestamp });
                e.HasIndex(ev => new { ev.EventType });

                e.Property(x => x.Payload).HasColumnType("jsonb");
            });
        }

        public DbSet<PersistedEvent> Events() => Set<PersistedEvent>();
    }
}
