<Query Kind="Program">
  <Connection>
    <ID>75df64b9-fa39-4b9a-990e-66f6ea8821c2</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Chinook</Database>
  </Connection>
</Query>

void Main()
{
	// pretend that the Main() is the web page
	
	//find songs by partial song name.
	//display the album title, song, and artist name.
	//order by song.
	
	//assume a value was entered into the web page
	//assume that a post button was pressed
	//assume Main() is the OnPost event
	
	string inputvalue ="dance";
	List<SongList> songCollection = SongsByPartialName(inputvalue);
	songCollection.Dump(); //assume is the web page display
}

// You can define other methods, fields, classes 
//and namespaces here

//C# really enjoyes strongly typed data fields
//	whether these fields are primitive data types (int, double, ...)
//	or developer defined datatypes (class)

public class SongList
{
	public string Album{get;set;}
	public string Song{get;set;}
	public string Artist{get;set;}
}

//imagine the following method exists in a service in your BLL
//this method receives the web page parameter value for the query
//this method will need to return a collection

List<SongList> SongsByPartialName(string partialSongName)
{
	IEnumerable<SongList> songCollection = Tracks
							.Where(t => t.Name.Contains(partialSongName))
							.OrderBy(t => t.Name)
							.Select(t => new SongList
								{
									Album = t.Album.Title,
									Song = t.Name,
									Artist = t.Album.Artist.Name
								}
							)
							;
	return songCollection.ToList();
}





