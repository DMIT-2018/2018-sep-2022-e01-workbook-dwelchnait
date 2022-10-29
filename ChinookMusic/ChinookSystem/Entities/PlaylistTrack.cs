using System;
using System.Collections.Generic;

namespace ChinookSystem.Entities
{
    internal partial class PlaylistTrack
    {
        public int PlaylistId { get; set; }
        public int TrackId { get; set; }
        public int TrackNumber { get; set; }

        public virtual Playlist Playlist { get; set; } = null!;
        public virtual Track Track { get; set; } = null!;
    }
}
