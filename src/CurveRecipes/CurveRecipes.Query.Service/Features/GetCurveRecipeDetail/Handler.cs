using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Events;
using Common.Core.Extensions;

namespace CurveRecipes.Query.Service.Features.GetCurveRecipeDetail
{
    public class Handler :
            IHandleQuery<Query, Maybe<Dto>>,
            IHandleEvent<CurveRecipeCreated>,
            IHandleEvent<KeyRateShockAdded>,
            IHandleEvent<ParallelShockAdded>,
            IHandleEvent<MarketCurveCreated>
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

        public async Task Handle(CurveRecipeCreated @event, CancellationToken cancellationToken)
        {
            await _marketCurveRepository
                .Get(@event.MarketCurveId)
                .ToResult()
                .Promise(curve =>
                {
                    var dto = new Dto
                    {
                        Id = @event.Id,
                        Name = GenerateName(@event, curve)
                    };

                    return _readModelRepository.Insert(dto);
                });
        }

        public Task<Maybe<Dto>> Handle(Query query, CancellationToken cancellationToken)
        {
            return _readModelRepository.Get(query.Id);
        }

        public async Task Handle(KeyRateShockAdded @event, CancellationToken cancellationToken)
        {
            var transformation = new TransformationDto
            {
                Name = KeyRateShock,
                Parameters = new List<ParameterDto>()
                    {
                        new ParameterDto
                        {
                            Name = nameof(@event.Shift),
                            Value = @event.Shift.ToString()
                        },
                        new ParameterDto
                        {
                            Name = nameof(@event.ShockTarget),
                            Value = @event.ShockTarget
                        },
                        new ParameterDto
                        {
                            Name = nameof(@event.Maturities),
                            Value = string.Join(';', @event.Maturities)
                        }
                    }
            };

            await _readModelRepository.Get(@event.Id)
                .ToResult()
                .Promise(recipe =>
                {
                    recipe.Transformations.Add(transformation);
                    UpdateName(recipe, transformation);
                    return _readModelRepository.Update(recipe);
                });
        }

        public async Task Handle(ParallelShockAdded @event, CancellationToken cancellationToken)
        {
            var transformation = new TransformationDto
            {
                Name = ParallelShock,
                Parameters = new List<ParameterDto>()
                    {
                        new ParameterDto
                        {
                            Name = nameof(@event.Shift),
                            Value = @event.Shift.ToString()
                        },
                        new ParameterDto
                        {
                            Name = nameof(@event.ShockTarget),
                            Value = @event.ShockTarget
                        }
                    }
            };

            await _readModelRepository.Get(@event.Id)
                .ToResult()
                .Promise(recipe =>
                {
                    recipe.Transformations.Add(transformation);
                    UpdateName(recipe, transformation);
                    return _readModelRepository.Update(recipe);
                });
        }

        public Task Handle(MarketCurveCreated @event, CancellationToken cancellationToken)
        {
            var dto = new MarketCurveNamePartDto
            {
                Id = @event.Id,
                Value = GenerateName(@event)
            };

            return _marketCurveRepository.Insert(dto);
        }

        private string GenerateName(MarketCurveCreated @event)
        {
            var stringBuilder = new StringBuilder(@event.Country);

            stringBuilder.AppendFormatNonEmptyString("_{0}", @event.CurveType, @event.FloatingLeg);

            return stringBuilder.ToString();
        }

        private string GenerateName(CurveRecipeCreated @event, MarketCurveNamePartDto marketCurvePart)
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
