using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Common.Core;
using CurveRecipes.Domain;

namespace CurveRecipes.Service.Features.AddTransformation
{
    public class Dto
    {
        public Dto(Guid id)
        {
            Commands = new Dictionary<string, ICommand>()
                {
                    { nameof(KeyRateShock), new AddShock.AddKeyRateShock.Command { Id = id } },
                    { nameof(ParallelShock), new AddShock.AddParallelShock.Command { Id = id } }
                }.ToImmutableDictionary();
        }

        public ImmutableArray<string> ShockTargets { get; } = Enum.GetNames(typeof(ShockTarget)).ToImmutableArray();

        public ImmutableDictionary<string, ICommand> Commands { get; }
    }
}
