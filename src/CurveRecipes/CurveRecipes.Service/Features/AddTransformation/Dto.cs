using System;
using System.Collections.Generic;
using CurveRecipes.Domain;

namespace CurveRecipes.Service.Features.AddTransformation
{
    public class Dto
    {
        public Dto(Guid id)
        {
            Id = id;
        }

        private void Add<T>(string name) where T : new()
        {
            Commands.Add(name, new T());
        }

        public Guid Id { get; }
        public IEnumerable<string> ShockTargets { get; } = Enum.GetNames(typeof(ShockTarget));

        public Dictionary<string, object> Commands { get; } = new Dictionary<string, object>
        {
            { nameof(KeyRateShock), new AddKeyRateShock() },
            { nameof(ParallelShock), new AddParallelShock() },
        };
    }
}
