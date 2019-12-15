using System;
using System.Collections.Generic;
using Common.Core;
using Google.Protobuf.WellKnownTypes;

namespace Common.Events
{
    public interface ICurveCalculationFailed : IEvent
    {
        DateTime AsAtDate { get; }
        string AsOfDate { get; }
        string CurveRecipeId { get; }
        IEnumerable<string> Messages { get; }
    }

    internal partial class CurveCalculationFailed : ICurveCalculationFailed
    {
        public CurveCalculationFailed(Guid aggregateId, Guid curveRecipeId, string asOfDate, DateTime asAtDate, string[] messages)
        {
            AggregateId = aggregateId.ToString();
            CurveRecipeId = curveRecipeId.ToString();
            AsOfDate = asOfDate;
            AsAtDate = Timestamp.FromDateTime(asAtDate.ToUniversalTime());
            Messages.Add(messages);
        }

        DateTime ICurveCalculationFailed.AsAtDate => AsAtDate.ToDateTime();

        IEnumerable<string> ICurveCalculationFailed.Messages => Messages;

        Guid IEvent.AggregateId => Guid.Parse(AggregateId);

        public IEvent WithVersion(int version)
        {
            var clone = (CurveCalculationFailed)MemberwiseClone();
            clone.Version = version;
            return clone;
        }
    }
}
