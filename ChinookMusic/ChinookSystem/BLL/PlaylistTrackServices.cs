#nullable disable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using ChinookSystem.DAL;
using ChinookSystem.Entities;
using ChinookSystem.ViewModels;
#endregion

namespace ChinookSystem.BLL
{
    public class PlaylistTrackServices
    {
        #region Constructor for Context Dependency
        private readonly ChinookContext _context;

        internal PlaylistTrackServices(ChinookContext context)
        {
            _context = context;
        }
        #endregion

        #region Queries
        public List<PlaylistTrackInfo> PlaylistTrack_FetchPlaylist(string playlistname, string username)
        {
            if (string.IsNullOrWhiteSpace(playlistname))
            {
                throw new ArgumentNullException("No playlist name submitted");
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException("No user name submitted");
            }
            IEnumerable<PlaylistTrackInfo> results = _context.PlaylistTracks
                                        .Where(x => x.Playlist.Name.Equals(playlistname)
                                                 && x.Playlist.UserName.Equals(username))
                                        .Select(x => new PlaylistTrackInfo
                                        {
                                            TrackId = x.TrackId,
                                            TrackNumber = x.TrackNumber,
                                            SongName = x.Track.Name,
                                            Milliseconds = x.Track.Milliseconds
                                        })
                                        .OrderBy(x => x.TrackNumber);
            return results.ToList();
        }

        #endregion

        #region Command TRX methods

        void PlaylistTrack_AddTrack(string playlistname, string username, int trackid)
        {
            //locals
            Track trackexists = null;
            Playlist playlistexists = null;
            PlaylistTrack playlisttrackexists = null;
            int tracknumber = 0;

            if (string.IsNullOrWhiteSpace(playlistname))
            {
                throw new ArgumentNullException("No playlist name submitted");
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException("No user name submitted");
            }

            trackexists = _context.Tracks
                            .Where(x => x.TrackId == trackid)
                            .Select(x => x)
                            .FirstOrDefault();
            if (trackexists == null)
            {
                throw new ArgumentException("Selected track no longer on file. Refresh track table.");
            }

            //B/R  playlist names must be unique within a user
            playlistexists = _context.Playlists
                                .Where(x => x.Name.Equals(playlistname)
                                        && x.UserName.Equals(username))
                                .Select(x => x)
                                .FirstOrDefault();
            if (playlistexists == null)
            {
                playlistexists = new Playlist()
                {
                    Name = playlistname,
                    UserName = username
                };
                //staging the new playlist record
                _context.Playlists.Add(playlistexists);
                tracknumber = 1;
            }
            else
            {
                // B/R a track may only exist once on a playlist
                playlisttrackexists = _context.PlaylistTracks
                                        .Where(x => x.Playlist.Name.Equals(playlistname)
                                                &&  x.Playlist.UserName.Equals(username)
                                                &&  x.TrackId == trackid)
                                        .Select(x => x)
                                        .FirstOrDefault();
                if (playlisttrackexists == null)
                {
                    //generate the next tracknumber
                    tracknumber = _context.PlaylistTracks
                                    .Where(x => x.Playlist.Name.Equals(playlistname)
                                                &&  x.Playlist.UserName.Equals(username))
                                    .Count();
                    tracknumber++;
                }
                else
                {
                    var songname = _context.Tracks
                                    .Where(x => x.TrackId == trackid)
                                    .Select(x => x.Name)
                                    .SingleOrDefault();
                    throw new Exception($"Selected track ({songname}) already exists on the playlist.");
                }

            }

            //processing to stage the new track to the playlist
            playlisttrackexists = new PlaylistTrack();

            //load the data to the new instance of playlist track
            playlisttrackexists.TrackNumber = tracknumber;
            playlisttrackexists.TrackId = trackid;

            /*******************************************
            ?? what about the second part of the primary key: PlaylistId
               it the playlist exists then we know the id:
                    playlistexists.PlaylistId;

            in the situation of a NEW playlist, even though we have
                created the playlist instance (see above) it is ONLY staged!!!

            this means that the actual sql records has NOT yet been created
            this means that the IDENTITY value for the new playlist DOES NOT 
                yet exists. The value on the playlist instance (playlistexists)
                is zero.
            thus we have a serious problem

            Solution
            it is built into EntityFramwework software and is based on using
                the navigational property in Playlists pointing to its "child"

            staging a typical Add in the past was to reference the entity
                and use the entity.Add(xxxxxx)
                _context.PlaylistTrack.Add(xxxxx)  [_context. is context instance in VS]
            IF you use this statement, the playlistid would be zero (0)
                causing your transaction to ABORT

            INSTEAD. do the staging using the syntax of "parent.navigationalproperty.Add(xxxxx)
            playlistexists will be filled with either
                scenario A) a new staged instance
                scenario B) a copy of the existing playlist instance
            *******************************************/
            playlistexists.PlaylistTracks.Add(playlisttrackexists);

            /**************************************************
            Staging is complete
            Commit the work (transaction)
            commiting the work needs a .SaveChanges()
            a transaction has ONLY ONE .SaveChanges()
            IF the SaveChanges() fails then all staged work being handled by the SaveChanges
                is rollback.

            *************************************************/
            _context.SaveChanges();
        }

        public void PlaylistTrack_RemoveTracks(string playlistname, string username,
                        List<PlaylistTrackTRX> tracklistinfo)
        {
            //local variables
            Playlist playlistexists = null;
            PlaylistTrack playlisttrackexists = null;
            int tracknumber = 0;

            //we need a container to hold x number of Exception messages
            List<Exception> errorlist = new List<Exception>();

            if (string.IsNullOrWhiteSpace(playlistname))
            {
                throw new ArgumentNullException("No playlist name submitted");
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException("No user name submitted");
            }

            var count = tracklistinfo.Count();
            if (count == 0)
            {
                throw new ArgumentNullException("No list of tracks were submitted");
            }

            playlistexists = _context.Playlists
                                .Where(x => x.Name.Equals(playlistname)
                                        && x.UserName.Equals(username))
                                .Select(x => x)
                                .FirstOrDefault();
            if (playlistexists == null)
            {
                errorlist.Add(new Exception($"Play list {playlistname} does not exist for this user."));
            }
            else
            {
                //obtain the tracks to keep
                //the SelectedTrack is a boolean field
                //	false: keep
                //	true : remove
                //create a query to extract the "keep" tracks from the incoming data
                IEnumerable<PlaylistTrackTRX> keeplist = tracklistinfo
                                                        .Where(x => !x.SelectedTrack)
                                                        .OrderBy(x => x.TrackNumber);
                //obtain the tracks to remove
                IEnumerable<PlaylistTrackTRX> removelist = tracklistinfo
                                                        .Where(x => x.SelectedTrack);

                foreach (PlaylistTrackTRX item in removelist)
                {
                    playlisttrackexists = _context.PlaylistTracks
                                        .Where(x => x.Playlist.Name.Equals(playlistname)
                                                && x.Playlist.UserName.Equals(username)
                                                && x.TrackId == item.TrackId)
                                        .FirstOrDefault();
                    if (playlisttrackexists != null)
                    {
                        _context.PlaylistTracks.Remove(playlisttrackexists);
                    }
                }

                tracknumber= 1;
                foreach (PlaylistTrackTRX item in keeplist)
                {
                    playlisttrackexists = _context.PlaylistTracks
                                        .Where(x => x.Playlist.Name.Equals(playlistname)
                                                && x.Playlist.UserName.Equals(username)
                                                && x.TrackId == item.TrackId)
                                        .FirstOrDefault();
                    if (playlisttrackexists != null)
                    {
                        playlisttrackexists.TrackNumber = tracknumber;
                        _context.PlaylistTracks.Update(playlisttrackexists);

                        //this library is not directly accessable by linqpad
                        //EntityEntry<PlaylistTracks> updating = _context.Entry(playlisttrackexists);
                        //updating.State = Microsoft.EntityFrameworkCore.EntityState.Modify;

                        //get ready for next track
                        tracknumber++;
                    }
                    else
                    {
                        var songname = _context.Tracks
                                    .Where(x => x.TrackId == item.TrackId)
                                    .Select(x => x.Name)
                                    .SingleOrDefault();
                        errorlist.Add(new Exception($"The track ({songname}) is no longer on file. Please Remove"));
                    }
                }


            }
            if (errorlist.Count > 0)
            {
                throw new AggregateException("Unable to remove request tracks. Check concerns", errorlist);
            }
            else
            {
                //all work has been staged
                _context.SaveChanges();
            }
        }

        public void PlaylistTrack_MoveTracks(string playlistname, string username,
                        List<PlaylistTrackTRX> tracklistinfo)
        {
            //local variables
            Playlist playlistexists = null;
            PlaylistTrack playlisttrackexists = null;
            int tracknumber = 0;

            //we need a container to hold x number of Exception messages
            List<Exception> errorlist = new List<Exception>();

            if (string.IsNullOrWhiteSpace(playlistname))
            {
                throw new ArgumentNullException("No playlist name submitted");
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException("No user name submitted");
            }

            var count = tracklistinfo.Count();
            if (count == 0)
            {
                throw new ArgumentNullException("No list of tracks were submitted");
            }

            playlistexists = _context.Playlists
                                .Where(x => x.Name.Equals(playlistname)
                                        && x.UserName.Equals(username))
                                .Select(x => x)
                                .FirstOrDefault();
            if (playlistexists == null)
            {
                errorlist.Add(new Exception($"Play list {playlistname} does not exist for this user."));
            }
            else
            {

                //validation loop to check that the data is indeed a postive number
                //use int.TryParse to check that the value to be tested is a number
                //check the result of tryparse against the value 1
                int tempnum = 0;
                foreach (var track in tracklistinfo)
                {
                    var songname = _context.Tracks
                                    .Where(x => x.TrackId == track.TrackId)
                                    .Select(x => x.Name)
                                    .SingleOrDefault();
                    if (int.TryParse(track.TrackInput.ToString(), out tempnum))
                    {
                        if (tempnum < 1)
                        {
                            errorlist.Add(new Exception($"The track ({songname}) re-sequence value needs to be greater than 0. Example: 3"));

                        }

                    }
                    else
                    {
                        errorlist.Add(new Exception($"The track ({songname}) re-sequence value needs to be a number. Example: 3"));
                    }
                }

                //sort the command model data list on the re-org value
                //	in ascending order  comparing x to y
                //   a descending order comparing y to x
                tracklistinfo.Sort((x, y) => x.TrackInput.CompareTo(y.TrackInput));

                //b) unique new track numbers
                // the collection has been sorted in ascending order therefore the next
                //		number must be equal to or greater than the previous number
                // one could check to see if the next number is +1 of the previou number
                //		BUT the re-org loop which does the actual re-sequence of numbers
                //		will have that situation.
                //		Therefore "holes" in this loop does not matter (logically)
                for (int i = 0; i < tracklistinfo.Count - 1; i++)
                {
                    var songname1 = _context.Tracks
                                    .Where(x => x.TrackId == tracklistinfo[i].TrackId)
                                    .Select(x => x.Name)
                                    .SingleOrDefault();
                    var songname2 = _context.Tracks
                                    .Where(x => x.TrackId == tracklistinfo[i + 1].TrackId)
                                    .Select(x => x.Name)
                                    .SingleOrDefault();
                    if (tracklistinfo[i].TrackInput == tracklistinfo[i+1].TrackInput)
                    {
                        errorlist.Add(new Exception($"{songname1} and {songname2} have the same re-sequence value. Re-sequence numbers must be unique"));
                    }
                }


                tracknumber= 1;
                foreach (PlaylistTrackTRX item in tracklistinfo)
                {
                    playlisttrackexists = _context.PlaylistTracks
                                        .Where(x => x.Playlist.Name.Equals(playlistname)
                                                && x.Playlist.UserName.Equals(username)
                                                && x.TrackId == item.TrackId)
                                        .FirstOrDefault();
                    if (playlisttrackexists != null)
                    {
                        playlisttrackexists.TrackNumber = tracknumber;
                        _context.PlaylistTracks.Update(playlisttrackexists);

                        //this library is not directly accessable by linqpad
                        //EntityEntry<PlaylistTracks> updating = _context.Entry(playlisttrackexists);
                        //updating.State = Microsoft.EntityFrameworkCore.EntityState.Modify;

                        //get ready for next track
                        tracknumber++;
                    }
                    else
                    {
                        var songname = _context.Tracks
                                    .Where(x => x.TrackId == item.TrackId)
                                    .Select(x => x.Name)
                                    .SingleOrDefault();
                        errorlist.Add(new Exception($"The track ({songname}) is no longer on file. Please Remove"));
                    }
                }


            }
            if (errorlist.Count > 0)
            {
                throw new AggregateException("Unable to re-sequence tracks. Check concerns", errorlist);
            }
            else
            {
                //all work has been staged
                _context.SaveChanges();
            }
        }

        #endregion

    }
}
