﻿using Microsoft.EntityFrameworkCore;

namespace Common.EventStore.Lib.EfCore
{
    internal class EventStoreContext : DbContext
    {
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
            });
        }

        public DbSet<PersistedEvent> Events() => Set<PersistedEvent>();
    }
}
