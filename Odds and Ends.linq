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
	//Conversions
	//collection we will look at are Iqueryable, IEnumerable and List
	
	//Display all albums and their tracks. Display the album title
	//artist name and album tracks. For each track show the song name
	//and play time. Show only albums with 25 or more tracks.
	
	List<AlbumTracks> albumlist = Albums.ToList()
					.Where(a => a.Tracks.Count >= 25)
					.Select( a => new AlbumTracks
					{
						Title = a.Title,
						Artist = a.Artist.Name,
						Songs = a.Tracks
								.Select(tr => new SongItem
								{
									Song = tr.Name,
									Playtime = tr.Milliseconds / 1000.0
								})
								.ToList()
					})
					.ToList()
					//.Dump()
					;
	
	//Using .FirstOrDefault()
	//first saw in CPSC1517 when check to see if a record existed in a BLL service method
	
	//Find the first album by Deep Purple
	var artistparam = "Deep Purrple";
	var resultsFOD = Albums
					.Where(a => a.Artist.Name.Equals(artistparam))
					.Select(a => a)
					.OrderBy(a => a.ReleaseYear)
					.FirstOrDefault()
					//.Dump()
					;
	//if (resultsFOD != null)
	//{
	//	resultsFOD.Dump();
	//}
	//else
	//{
	//	Console.WriteLine($"No albums found for artist {artistparam}");
	//}
	
	//Distinct()
	//remove duplicate reported lines
	
	//Get a list of customer countries
	var resultsDistinct = Customers
							.OrderBy(c => c.Country)
							.Select(c => c.Country)
							.Distinct()
							
							//.Dump()
							;
	
	//.Take() and .Skip()
	//in CPSC1517, when you want to your the supplied Paginator
	//	the query method was to return ONLY the need
	//	records for the display NOT the entire collection
	// a) the query was executed returning a collection of size x
	// b) obtained the total count (x) of return records
	// c) calculated the number of records to skip (pagenumber - 1) * pagesize
	// d) on the return method statement you used
	//		return variablename.Skip(rowsSkiped).Take(pagesize).ToList()
	
	//Union
	//rules in linq are the same as sql
	//result is the same a sql, combine separate collections into one.
	//syntax   (queryA).Union(queryB)[.Union(query....)]
	//rules:
	//	number of columns the same
	//	column dataypes must be the same
	//	ordering should be done as a method after the last Union
	
	var resultsUnionA = (Albums
								.Where(x => x.Tracks.Count() == 0)
								.Select(x => new
								{
									title = x.Title,
									totalTracks = 0,
									totalCost = 0.00m,
									averageLength = 0.00d
								})
								)
						
						.Union(Albums
						.Where(x => x.Tracks.Count() > 0)
						.Select(x => new
						{
							title = x.Title,
							totalTracks = x.Tracks.Count(),
							totalCost = x.Tracks.Sum(tr => tr.UnitPrice),
							averageLength = x.Tracks.Average(tr => tr.Milliseconds)
						})
						)
						.OrderBy(x => x.totalTracks)
						.Dump()
						;					
}

public class SongItem
{
	public string Song{get;set;}
	public double Playtime{get;set;}
}
public class AlbumTracks
{
	public string Title{get;set;}
	public string Artist {get;set;}
	public List<SongItem> Songs{get;set;}
}
// You can define other methods, fields, classes and namespaces here


















