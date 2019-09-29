using System;
using Common.Core;
using PricePublisher.Domain;

namespace PricePublisher.Service.Features.PublishPrice
{
    public class Command : ICommand
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime AsOfDate { get; set; } = DateTime.UtcNow.Date.AddDays(-1);
        public Guid InstrumentId { get; set; }
        public string PriceCurrency { get; set; }
        public double PriceAmount { get; set; }
        public PriceType? PriceType { get; set; }
    }
}
