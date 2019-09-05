using Common.Core;
using Common.Core.Events;
using Common.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarketCurves.Query.Service.Features
{
    public static class GetMarketCurveList
    {
        public class Result
        {
            public Result(IEnumerable<Dto> curves)
            {
                Curves = curves ?? throw new ArgumentNullException(nameof(curves));
            }

            public IEnumerable<Dto> Curves { get;}
        }

        public class Query : IQuery<Result>
        {
        }

        public class Dto : ReadObject
        {
            public string Name { get; set; }
        }

        public class Handler :
            IHandleQuery<Query, Result>,
            IHandleEvent<MarketCurveCreated>
        {
            private readonly IReadModelRepository<Dto> _readModelRepository;

            public Handler(IReadModelRepository<Dto> readModelRepository)
            {
                _readModelRepository = readModelRepository ?? throw new ArgumentNullException(nameof(readModelRepository));
            }

            public async Task<Result> Handle(Query query, CancellationToken cancellationToken)
            {
                var dtos = await _readModelRepository.GetAll();
                return new Result(dtos);
            }

            public Task Handle(MarketCurveCreated @event, CancellationToken cancellationToken)
            {
                var curve = new Dto
                {
                    Id = @event.Id,
                    Name = GenerateName(@event)
                };

                return _readModelRepository.Insert(curve);
            }

            private string GenerateName(MarketCurveCreated @event)
            {
                var stringBuilder = new StringBuilder("M");

                stringBuilder.AppendFormatNonEmptyString("_{0}", @event.Country, @event.CurveType, @event.FloatingLeg);

                return stringBuilder.ToString();
            }
        }
    }
}
