using System;
using System.Collections.Generic;

namespace CalculationEngine.Service
{
    public partial class EventJournal
    {
        public long Ordering { get; set; }
        public string PersistenceId { get; set; }
        public long SequenceNr { get; set; }
        public long Timestamp { get; set; }
        public bool IsDeleted { get; set; }
        public string Manifest { get; set; }
        public byte[] Payload { get; set; }
        public string Tags { get; set; }
        public int? SerializerId { get; set; }
    }
}
