<Query Kind="Expression">
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

//Aggregates
//.Count() counts the number of instances in the collection
//.Sum(x => ....) sums (totals) a numeric field (numeric expression) in a collection
//.Min(x => ...) finds the minimum value of a collection for a field
//.Max(x => ...) finds the maximum value of a collection for a field
//.Average(x => ...) finds the average value of a numeric field (numeric expression) in a collection

//IMPORTANT!!!!!!!!
//Aggregates work ONLY on a collection of values
//Aggregates DO NOT work on a single instance (non declare collection)

//.Sum, .Min, .Max and .Average MUST on at least on record in their collection
//.Sum and .Average MUST work on numeric fields and the field CANNOT be null

//syntax: method
//collectionset.aggregate(x => expression)
//collectionset.Select(...).aggregate()
//collectionset.Count() //.Coutn() does not contain an expression

//for Sum, Min, Max and Average: the result is a single value

//you can use multiple aggregates on a single column
//   .Sum(x => expression).Min(x => expression)

//Find the average playing time (length) of tracks in our music
//   collection

//thought process
//average is an aggregate
//what is the collection? the Tracks table is a collection
//what is the expression? Milliseconds

Tracks.Average(x => x.Milliseconds) //each x has multiple fields
Tracks.Select(x => x.Milliseconds).Average() //a single list of numbers
//Tracks.Average() aborts because no specific field was referred to
//		on the track record

//List all albums of the 60s showing the title artist and various
//	aggregates for albums containing tracks

//For each album show the number of tracks, the total price of
//	all tracks and the average playing length of the album tracks

//thought process
//start at Albums
//can I get the artist name (.Artist)
//can I get a collection of tracks for an album (x.Tracks)
//can I get the number of tracks in the collection (.Count())
//can I get the total price of the tracks (.Sum())
//can I get the average of the play length (.Average())

Albums
 	.Where(x => x.Tracks.Count() > 0 
			&& (x.ReleaseYear > 1959 && x.ReleaseYear < 1970))
	.Select(x => new
	{
		Title = x.Title,
		Artist = x.Artist.Name,
		NumberofTracks = x.Tracks.Count(),
		TotalPrice = x.Tracks.Sum(tr => tr.UnitPrice),
		AverageTrackLength = x.Tracks.Select(tr => tr.Milliseconds).Average()
	})












