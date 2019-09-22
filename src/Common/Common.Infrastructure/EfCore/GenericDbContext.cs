using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Core;
using Common.Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.EfCore
{
    public class GenericDbContext : DbContext
    {
        private readonly IEnumerable<Type> _readModelTypes;

        public GenericDbContext(DbContextOptions options, IEnumerable<Type> types) : base(options)
        {
            _readModelTypes = types;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EventPosition>();

            foreach (var readModelType in _readModelTypes)
            {
                var entity = modelBuilder.Entity(readModelType);

                var classProperties = readModelType.GetProperties().Where(x => x.PropertyType.IsClass && x.PropertyType.AssemblyQualifiedName == readModelType.AssemblyQualifiedName);

                foreach (var prop in classProperties)
                {
                    entity.OwnsOne(prop.PropertyType, prop.Name);
                }
            }
        }
    }
}
