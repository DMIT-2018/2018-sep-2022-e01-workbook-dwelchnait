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

//Sorting

//there is a significant difference between query syntax
//	and method syntax

//query syntax is much like sql
//  orderby field {[ascending]|descending} [,field ....]

// ascending is the default option

//method syntax is a series of individual methods
// .OrderBy(x => x.field)  first field ONLY
// .OrderByDescending(x = x.field) first field ONLY
// .ThenBy(x = > x.field) each following field
// .ThenByDescending(x => x.field) each following field

//Find all of the album tracks for the band Queen. Order
//	the track by the track name alphabetically

//query syntax
from x in Tracks
where x.Album.Artist.Name.Contains("Queen")
orderby x.AlbumId, x.Name 
select x

//mehtod syntax
Tracks
 .Where (x => x.Album.Artist.Name.Contains("Queen"))
 .OrderBy(x => x.Album.Title)
 .ThenBy(x => x.Name)

//order of sorting and filter can be interchanged
Tracks
 .OrderBy(x => x.Album.Title)
 .ThenBy(x => x.Name)
 .Where (x => x.Album.Artist.Name.Contains("Queen"))





