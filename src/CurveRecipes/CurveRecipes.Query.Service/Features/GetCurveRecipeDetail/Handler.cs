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

        public async Task Handle(IEventWrapper<ICurveRecipeCreated> @event, CancellationToken cancellationToken)
        {
            await _marketCurveRepository
                .Get(@event.GetContent().MarketCurveId)
                .ToResult()
                .Promise(curve =>
                {
                    var dto = new Dto
                    {
                        Id = @event.Metadata.AggregateId,
                        Name = GenerateName(@event.GetContent(), curve)
                    };

                    return _readModelRepository.Insert(dto);
                });
        }

        public Task<Dto?> Handle(Query query, CancellationToken cancellationToken)
        {
            return _readModelRepository.Get(query.Id);
        }

        public async Task Handle(IEventWrapper<IKeyRateShockAdded> @event, CancellationToken cancellationToken)
        {
            var transformation = new TransformationDto
            {
                Name = KeyRateShock,
                Parameters = new List<ParameterDto>()
                    {
                        new ParameterDto
                        {
                            Name = nameof(@event.GetContent().Shift),
                            Value = @event.GetContent().Shift.ToString()
                        },
                        new ParameterDto
                        {
                            Name = nameof(@event.GetContent().ShockTarget),
                            Value = @event.GetContent().ShockTarget
                        },
                        new ParameterDto
                        {
                            Name = nameof(@event.GetContent().Maturities),
                            Value = string.Join(';', @event.GetContent().Maturities)
                        }
                    }
            };

            await _readModelRepository.Get(@event.Metadata.AggregateId)
                .ToResult()
                .Promise(recipe =>
                {
                    recipe.Transformations.Add(transformation);
                    UpdateName(recipe, transformation);
                    return _readModelRepository.Update(recipe);
                });
        }

        public async Task Handle(IEventWrapper<IParallelShockAdded> @event, CancellationToken cancellationToken)
        {
            var transformation = new TransformationDto
            {
                Name = ParallelShock,
                Parameters = new List<ParameterDto>()
                    {
                        new ParameterDto
                        {
                            Name = nameof(@event.GetContent().Shift),
                            Value = @event.GetContent().Shift.ToString()
                        },
                        new ParameterDto
                        {
                            Name = nameof(@event.GetContent().ShockTarget),
                            Value = @event.GetContent().ShockTarget
                        }
                    }
            };

            await _readModelRepository.Get(@event.Metadata.AggregateId)
                .ToResult()
                .Promise(recipe =>
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
                Id = @event.Metadata.AggregateId,
                Value = GenerateName(@event.GetContent())
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
