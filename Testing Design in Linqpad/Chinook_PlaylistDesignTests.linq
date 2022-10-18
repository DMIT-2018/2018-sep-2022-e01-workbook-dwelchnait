<Query Kind="Program">
  <Connection>
    <ID>5cd86f66-469b-4208-8b6f-a9f6ebe611e6</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <Server>.\SQLEXPRESS</Server>
    <Database>Chinook</Database>
    <DisplayName>Chinook-Entity</DisplayName>
    <DriverData>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
    </DriverData>
  </Connection>
</Query>

void Main()
{
	//Main is going to represent the web page post method
	try
	{
		//coded and tested the FetchTracksBy query
		string searcharg ="Deep";
		string searchby = "Artist";
		List<TrackSelection> tracklist = Track_FetchTracksBy(searcharg, searchby);
		//tracklist.Dump();
		
		//coded and tested the FetchPlaylist query
		string playlistname ="hansenb1";
		string username = "HansenB"; //this is an user name which will come from O/S via security
		List<PlaylistTrackInfo> playlist = PlaylistTrack_FetchPlaylist(playlistname, username);
		//playlist.Dump();
		
		//coded and tested the Add_Track trx
		//the command method will receive no collection but will receive individual arguments
		// trackid, playlistname, username
		//test tracks
		//793 A castle full of Rascals
		//822 A Twist in the Tail
		//543 Burn
		//756 Child in Time
		
		//on the web page, the post method would have already have access to the
		//  BindProperty variables containing the input values
		playlistname = "hansenbtest";
		int trackid = 756;

		//call the service method to process the data
		PlaylistTrack_AddTrack(playlistname, username, trackid);
		
		//once the service method is complete, the web page would refresh
		playlist = PlaylistTrack_FetchPlaylist(playlistname, username);
		playlist.Dump();
	}
	catch (ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	catch (ArgumentException ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	catch (Exception ex)
	{
		GetInnerException(ex).Message.Dump();
	}
}

// You can define other methods, fields, classes and namespaces here

#region CQRS Queries
public class TrackSelection
{
    public int TrackId {get; set;}
    public string SongName {get; set;}
    public string AlbumTitle{get; set;}
    public string ArtistName{get; set;}
    public int Milliseconds {get; set;}
    public decimal Price {get; set;}
}
public class PlaylistTrackInfo 
{
    public int TrackId {get; set;}
    public int TrackNumber {get; set;}
    public string SongName {get; set;}
    public int Milliseconds {get; set;}
}
#endregion

//general method to drill down into an exception of obtain the InnerException where your
//  actual error is detailed

private Exception GetInnerException(Exception ex)
{
	while (ex.InnerException != null)
		ex = ex.InnerException;
	return ex;
}


//pretend to be the class library project
#region TrackServices class
public List<TrackSelection> Track_FetchTracksBy(string searcharg, string searchby)
{
	if (string.IsNullOrWhiteSpace(searcharg))
	{
		throw new ArgumentNullException("No search value submitted");
	}
	if (string.IsNullOrWhiteSpace(searchby))
	{
		throw new ArgumentNullException("No search style submitted");
	}
	IEnumerable<TrackSelection> results = Tracks
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
										});
	return results.ToList();
}

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
	IEnumerable<PlaylistTrackInfo> results = PlaylistTracks
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
	Tracks trackexists = null;
	Playlists playlistexists = null;
	PlaylistTracks playlisttrackexists = null;
	int tracknumber =0;
	
	if (string.IsNullOrWhiteSpace(playlistname))
	{
		throw new ArgumentNullException("No playlist name submitted");
	}
	if (string.IsNullOrWhiteSpace(username))
	{
		throw new ArgumentNullException("No user name submitted");
	}
	
	trackexists = Tracks
					.Where(x => x.TrackId == trackid)
					.Select(x => x)
					.FirstOrDefault();
	if (trackexists == null)
	{
		throw new ArgumentException("Selected track no longer on file. Refresh track table.");
	}
	
	//B/R  playlist names must be unique within a user
	playlistexists = Playlists
						.Where(x => x.Name.Equals(playlistname)
								&& x.UserName.Equals(username))
						.Select(x => x)
						.FirstOrDefault();
	if (playlistexists == null)
	{
		playlistexists = new Playlists()
		{
			Name = playlistname,
			UserName = username
		};
		//staging the new playlist record
		Playlists.Add(playlistexists);
		tracknumber = 1;
	}
	else
	{
		// B/R a track may only exist once on a playlist
		playlisttrackexists = PlaylistTracks
								.Where(x => x.Playlist.Name.Equals(playlistname)
										&&  x.Playlist.UserName.Equals(username)
										&&  x.TrackId == trackid)
								.Select(x => x)
								.FirstOrDefault();
		if (playlisttrackexists == null)
		{
			//generate the next tracknumber
			tracknumber = PlaylistTracks
							.Where(x => x.Playlist.Name.Equals(playlistname)
										&&  x.Playlist.UserName.Equals(username))
							.Count();
			tracknumber++;
		}
		else
		{
			var songname = Tracks
							.Where(x => x.TrackId == trackid)
							.Select( x => x.Name)
							.SingleOrDefault();
			throw new Exception($"Selected track ({songname}) already exists on the playlist.");
		}
										
	}
	
	//processing to stage the new track to the playlist
	playlisttrackexists = new PlaylistTracks();
	
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
	SaveChanges();
}
#endregion













