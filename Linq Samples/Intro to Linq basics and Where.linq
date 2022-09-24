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

Albums

//query syntax to list all records in an entity (table, collection)
from arowoncollection in Albums
select arowoncollection

//method syntax to list all records in an entity
Albums
   .Select (arowoncollection => arowoncollection)

//Where
//filter method
//the conditions are setup as you would in C#
//beware that Linqpad may NOT like some C# syntax (DateTime)
//beware that Linq is converted to Sql which may not
//  like certain C# syntax because Sql count not convert

//syntax
//notice tha the method syntax makes use of the Lambda expessions
//Lambdas are common when performing Linq with the method syntax
//.Where(lamba expression)
//.Where( x => condition [logical operator condition2 ....])
//.Where(x => x.Bytes > 350000)

Tracks
	.Where(x => x.Bytes > 700000000)

from x in Tracks
where x.Bytes > 700000000
select x

//Find all the albums of the artist Queen.
//concerns: the artist name is in another table
//			in an sql Select you would be using an inner Join
//			in LInq you DO MOT need to specific your inner Join
//			instead use the "navigational properties" of your entity
//				to generate the relationship
Albums
	.Where(a => a.Artist.Name.Contains("Queen"))




