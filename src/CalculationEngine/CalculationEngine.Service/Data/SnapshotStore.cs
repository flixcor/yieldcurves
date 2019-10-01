using System;
using System.Collections.Generic;

namespace CalculationEngine.Service
{
    public partial class SnapshotStore
    {
        public string PersistenceId { get; set; }
        public long SequenceNr { get; set; }
        public DateTime Timestamp { get; set; }
        public string Manifest { get; set; }
        public byte[] Snapshot { get; set; }
        public int? SerializerId { get; set; }
    }
}
