using System;
using System.Collections.Generic;

namespace ChinookSystem.Entities
{
    internal partial class Album
    {
        public Album()
        {
            Tracks = new HashSet<Track>();
        }

        public int AlbumId { get; set; }
        public string Title { get; set; } = null!;
        public int ArtistId { get; set; }
        public int ReleaseYear { get; set; }
        public string? ReleaseLabel { get; set; }

        public virtual Artist Artist { get; set; } = null!;
        public virtual ICollection<Track> Tracks { get; set; }
    }
}
