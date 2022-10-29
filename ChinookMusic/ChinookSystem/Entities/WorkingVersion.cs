using System;
using System.Collections.Generic;

namespace ChinookSystem.Entities
{
    internal partial class WorkingVersion
    {
        public int VersionId { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }
        public int Revision { get; set; }
        public DateTime AsOfDate { get; set; }
        public string? Comments { get; set; }
    }
}
