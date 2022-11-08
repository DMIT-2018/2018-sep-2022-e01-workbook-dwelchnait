#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using ChinookSystem.DAL;
using ChinookSystem.ViewModels;
#endregion

namespace ChinookSystem.BLL
{
    public class TrackServices
    {

        #region Constructor for Context Dependency
        private readonly ChinookContext _context;

        internal TrackServices(ChinookContext context)
        {
            _context = context;
        }
        #endregion

        #region Queries
        public List<TrackSelection> Track_FetchTracksBy(string searcharg, 
            string searchby, int pagenumber, int pagesize, out int totalcount)
        {
            if (string.IsNullOrWhiteSpace(searcharg))
            {
                throw new ArgumentNullException("No search value submitted");
            }
            if (string.IsNullOrWhiteSpace(searchby))
            {
                throw new ArgumentNullException("No search style submitted");
            }
            IEnumerable<TrackSelection> results = _context.Tracks
                                        .Where(x => (x.Album.Artist.Name.Contains(searcharg) &&
                                                    searchby.Equals("Artist")) ||
                                                    (x.Album.Title.Contains(searcharg) &&
                                                    searchby.Equals("Album")))
                                        .Select(x => new TrackSelection
                                        {
                                            TrackId = x.TrackId,
                                            SongName = x.Name,
                                            AlbumTitle = x.Album.Title,
                                            ArtistName = x.Album.Artist.Name,
                                            Milliseconds = x.Milliseconds,
                                            Price = x.UnitPrice
                                        })
                                        .OrderBy(x => x.SongName);
            totalcount = results.Count();
            int rowsskipped = (pagenumber - 1) * pagesize;

            return results.Skip(rowsskipped).Take(pagesize).ToList();
        }

        #endregion
    }
}
