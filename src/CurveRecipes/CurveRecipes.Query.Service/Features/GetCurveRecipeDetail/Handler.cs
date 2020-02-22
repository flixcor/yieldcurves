using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Core.Extensions;
using Common.Events;

namespace CurveRecipes.Query.Service.Features.GetCurveRecipeDetail
{
    public class Handler :
            IHandleQuery<Query, Dto?>,
            IHandleEvent<ICurveRecipeCreated>,
            IHandleEvent<IKeyRateShockAdded>,
            IHandleEvent<IParallelShockAdded>,
            IHandleEvent<IMarketCurveCreated>
    {
        private const string ParallelShock = "ParallelShock";
        private const string KeyRateShock = "KeyRateShock";

        private readonly IReadModelRepository<Dto> _readModelRepository;
        private readonly IReadModelRepository<MarketCurveNamePartDto> _marketCurveRepository;

        public Handler(IReadModelRepository<Dto> readModelRepository, IReadModelRepository<MarketCurveNamePartDto> marketCurveRepository)
        {
            _readModelRepository = readModelRepository ?? throw new ArgumentNullException(nameof(readModelRepository));
            _marketCurveRepository = marketCurveRepository ?? throw new ArgumentNullException(nameof(marketCurveRepository));
        }

        public Task Handle(IEventWrapper<ICurveRecipeCreated> @event, CancellationToken cancellationToken)
        {
            return _marketCurveRepository
                .Get(@event.Content.MarketCurveId.NonEmpty())
                .IfNotNull(curve =>
                {
                    var dto = new Dto
                    {
                        Id = @event.AggregateId,
                        Name = GenerateName(@event.Content, curve)
                    };

                    return _readModelRepository.Insert(dto);
                });
        }

        public Task<Dto?> Handle(Query query, CancellationToken cancellationToken)
        {
            return _readModelRepository.Get(query.Id.NonEmpty());
        }

        public Task Handle(IEventWrapper<IKeyRateShockAdded> @event, CancellationToken cancellationToken)
        {
            var transformation = new TransformationDto
            {
                Name = KeyRateShock,
                Parameters = new List<ParameterDto>()
                    {
                        new ParameterDto
                        {
                            Name = nameof(@event.Content.Shift),
                            Value = @event.Content.Shift.ToString()
                        },
                        new ParameterDto
                        {
                            Name = nameof(@event.Content.ShockTarget),
                            Value = @event.Content.ShockTarget
                        },
                        new ParameterDto
                        {
                            Name = nameof(@event.Content.Maturities),
                            Value = string.Join(';', @event.Content.Maturities)
                        }
                    }
            };

            return _readModelRepository.Get(@event.AggregateId)
                .IfNotNull(recipe =>
                {
                    recipe.Transformations.Add(transformation);
                    UpdateName(recipe, transformation);
                    return _readModelRepository.Update(recipe);
                });
        }

        public Task Handle(IEventWrapper<IParallelShockAdded> @event, CancellationToken cancellationToken)
        {
            var transformation = new TransformationDto
            {
                Name = ParallelShock,
                Parameters = new List<ParameterDto>()
                    {
                        new ParameterDto
                        {
                            Name = nameof(@event.Content.Shift),
                            Value = @event.Content.Shift.ToString()
                        },
                        new ParameterDto
                        {
                            Name = nameof(@event.Content.ShockTarget),
                            Value = @event.Content.ShockTarget
                        }
                    }
            };

            return _readModelRepository.Get(@event.AggregateId)
                .IfNotNull(recipe =>
                {
                    recipe.Transformations.Add(transformation);
                    UpdateName(recipe, transformation);
                    return _readModelRepository.Update(recipe);
                });
        }

        public Task Handle(IEventWrapper<IMarketCurveCreated> @event, CancellationToken cancellationToken)
        {
            var dto = new MarketCurveNamePartDto
            {
                Id = @event.AggregateId,
                Value = GenerateName(@event.Content)
            };

            return _marketCurveRepository.Insert(dto);
        }

        private string GenerateName(IMarketCurveCreated @event)
        {
            var stringBuilder = new StringBuilder(@event.Country);

            stringBuilder.AppendFormatNonEmptyString("_{0}", @event.CurveType, @event.FloatingLeg);

            return stringBuilder.ToString();
        }

        private string GenerateName(ICurveRecipeCreated @event, MarketCurveNamePartDto marketCurvePart)
        {
            var stringBuilder = new StringBuilder("C");

            stringBuilder.AppendFormatNonEmptyString("_{0}", marketCurvePart.Value, @event.DayCountConvention);

            return stringBuilder.ToString();
        }

        private void UpdateName(Dto dto, TransformationDto transformationDto)
        {
            var stringBuilder = new StringBuilder(dto.Name);

            stringBuilder.AppendFormatNonEmptyString("_{0}", transformationDto.Name);

            dto.Name = stringBuilder.ToString();
        }
    }
}
