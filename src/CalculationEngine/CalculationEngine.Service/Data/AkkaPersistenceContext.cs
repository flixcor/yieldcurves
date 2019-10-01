using System;
using System.Collections.Generic;
using Common.Infrastructure.EfCore;
using Microsoft.EntityFrameworkCore;

namespace CalculationEngine.Service
{
    public class AkkaPersistenceContext : GenericDbContext
    {
        public AkkaPersistenceContext(DbContextOptions options, IEnumerable<Type> types) : base(options, types)
        {
        }

        public virtual DbSet<EventJournal> EventJournal { get; set; }
        public virtual DbSet<Metadata> Metadata { get; set; }
        public virtual DbSet<SnapshotStore> SnapshotStore { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EventJournal>(entity =>
            {
                entity.HasKey(e => e.Ordering);

                entity.HasIndex(e => new { e.PersistenceId, e.SequenceNr })
                    .HasName("QU_EventJournal")
                    .IsUnique();

                entity.Property(e => e.Manifest)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Payload).IsRequired();

                entity.Property(e => e.PersistenceId)
                    .IsRequired()
                    .HasColumnName("PersistenceID")
                    .HasMaxLength(255);

                entity.Property(e => e.Tags).HasMaxLength(100);
            });

            modelBuilder.Entity<Metadata>(entity =>
            {
                entity.HasKey(e => new { e.PersistenceId, e.SequenceNr });

                entity.Property(e => e.PersistenceId)
                    .HasColumnName("PersistenceID")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<SnapshotStore>(entity =>
            {
                entity.HasKey(e => new { e.PersistenceId, e.SequenceNr });

                entity.Property(e => e.PersistenceId)
                    .HasColumnName("PersistenceID")
                    .HasMaxLength(255);

                entity.Property(e => e.Manifest)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Snapshot).IsRequired();
            });
        }
    }
}
