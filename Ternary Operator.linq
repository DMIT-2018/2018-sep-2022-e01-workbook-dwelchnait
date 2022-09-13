<Query Kind="Expression">
  <Connection>
    <ID>843536d2-30c5-46a3-b0c6-b6bf56c69058</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>WB320-99\SQLEXPRESS</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Chinook</Database>
  </Connection>
</Query>

//List all albums by release label. Any album with no label
// should be indicated as Unknown
//List Title, Label and Artist Name
//Order by ReleaseLabel

//understand the problem
//	collection: albums
//	selective data: anonymous data set
//	label (nullable): either Unknown or label name *****
//  order by the release label field

//design
// Albums
// Select (new{})
// fields: title
//			label  ???? ternary operator (condition(s) ? true value : false value)
//			Artist.Name

//coding and testing
Albums
	//.OrderBy(x => x.ReleaseLabel)
	.Select(x => new
	{
		Title = x.Title,
		Label = x.ReleaseLabel == null ? "Unknown" : x.ReleaseLabel,
		Artist = x.Artist.Name
	}
	)
	.OrderBy(x => x.Label)
	
	
//List all albums showing the Title, Artist Name, Year and decade of
//	release using oldies, 70s, 80s, 90s or modern.
//Order by decade.

//Hint: can you have nested ternary operators? yes 

// < 1970
//    oldies
// else 
//  ( <1980 then 70's
//     else 
//      (< 1990 then 80's
//        else
//        (< 2000 then 90's
//         else
//           modern)))

Albums
	.Select(a => new
	{
		Title = a.Title,
		Artist = a.Artist.Name,
		Year = a.ReleaseYear,
		Decade = a.ReleaseYear < 1970 ? "Oldies" :
					a.ReleaseYear < 1980 ? "70s" :
					a.ReleaseYear < 1990 ? "80s" :
					a.ReleaseYear < 2000 ? "90s" : "Modern"
	})
	.OrderBy(a => a.Year)






	