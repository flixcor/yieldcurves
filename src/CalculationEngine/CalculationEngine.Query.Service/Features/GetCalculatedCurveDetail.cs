using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Core.Events;
using Common.Infrastructure.EfCore;
using Microsoft.EntityFrameworkCore;

namespace CalculationEngine.Query.Service.Features
{
    public class GetCalculatedCurveDetail : IQuery<Maybe<GetCalculatedCurveDetail.Dto>>
    {
        public Guid CurveRecipeId { get; set; }
        public DateTime AsOfDate { get; set; }

        public class Handler :
            IHandleQuery<GetCalculatedCurveDetail, Maybe<Dto>>,
            IHandleEvent<CurveCalculated>,
            IHandleEvent<CurveRecipeCreated>
        {
            private readonly GenericDbContext _db;

            public Handler(GenericDbContext db)
            {
                _db = db;
            }

            public async Task<Maybe<Dto>> Handle(GetCalculatedCurveDetail query, CancellationToken cancellationToken)
            {
                var res = await _db.Set<Dto>()
                    .Include(x => x.Points)
                    .FirstOrDefaultAsync(x => x.CurveRecipeId == query.CurveRecipeId && x.AsOfDate == query.AsOfDate);

                if (res != null)
                {
                    res.Points = res.Points.OrderBy(x => x.Maturity).ToImmutableArray();
                }
                
                return res.Maybe();
            }

            public async Task Handle(CurveCalculated @event, CancellationToken cancellationToken)
            {
                var recipe = await _db.FindAsync<CurveRecipe>(@event.CurveRecipeId);

                _db.Add(new Dto 
                { 
                    Id = @event.Id,
                    AsAtDate = @event.AsAtDate,
                    AsOfDate = @event.AsOfDate,
                    CurveRecipeId = @event.CurveRecipeId,
                    CurveRecipeName = recipe?.Name,
                    Points = @event.Points.Select(p => new Dto.Point
                    {
                        Id = Guid.NewGuid(),
                        Currency = p.Currency,
                        Maturity = p.Maturity,
                        Value = p.Value
                    }).ToImmutableArray()
                });
            }

            public Task Handle(CurveRecipeCreated @event, CancellationToken cancellationToken)
            {
                _db.Add(new CurveRecipe
                {
                    Id = @event.Id,
                    Name = @event.ShortName
                });

                return Task.CompletedTask;
            }
        }

        public class CurveRecipe : ReadObject
        {
            public string Name { get; set; }
        }

        public class Dto : ReadObject
        {
            public Guid CurveRecipeId { get; set; }
            public string CurveRecipeName { get; set; }
            public DateTime AsOfDate { get; set; }
            public DateTime AsAtDate { get; set; }
            public ICollection<Point> Points { get; set; }

            public class Point : ReadObject
            {
                public double Maturity { get; set; }
                public string Currency { get; set; }
                public double Value { get; set; }
            }
        }
    }
}
