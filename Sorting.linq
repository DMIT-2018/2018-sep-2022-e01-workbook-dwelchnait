<Query Kind="Expression">
  <Connection>
    <ID>54bf9502-9daf-4093-88e8-7177c12aaaaa</ID>
    <NamingService>2</NamingService>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <AttachFileName>&lt;ApplicationData&gt;\LINQPad\ChinookDemoDb.sqlite</AttachFileName>
    <DisplayName>Demo database (SQLite)</DisplayName>
    <DriverData>
      <PreserveNumeric1>true</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.Sqlite</EFProvider>
      <MapSQLiteDateTimes>true</MapSQLiteDateTimes>
      <MapSQLiteBooleans>true</MapSQLiteBooleans>
    </DriverData>
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





