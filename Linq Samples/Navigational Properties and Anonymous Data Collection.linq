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

//Using Navigational Properties and Anonymous data set (collection)

//reference: Student Notes/Demo/eRestaurant/Linq: Query and Method Syntax

//Find all ablums released in the 90's (1990-1999)
//Order the albums by ascending year and then alphabetically by album title
//Display the Year, Title, Artist Name and Release label

//concerns: a) not all properties of Album are to be displayed
//			b) the order of the properties are to be displayed
//				in a different sequence then the defintion of the
//				properties on the entity Album
//			c) the artist name is NOT on the Album table BUT is on
//				the Artist table

//solution: use an anonymous data collection

//the anonymous data instance is defined within the Select by
//		declared fields (properties)
//the order of the fields on the new defined instance will be
//		done in specifying the properties of the anonymous data collection

Albums
	.Where(x => x.ReleaseYear > 1989 && x.ReleaseYear < 2000)
	//.OrderBy(x => x.ReleaseYear)
	//.ThenBy(x => x.Title)
	.Select(x => new
		{
			Year = x.ReleaseYear,
			Title = x.Title,
			Artist = x.Artist.Name,
			Label = x.ReleaseLabel
		})
	.OrderBy(x => x.Year) //Year is in the anonymous data type definintion, ReleaseYear is NOT
	.ThenBy(x => x.Title)









