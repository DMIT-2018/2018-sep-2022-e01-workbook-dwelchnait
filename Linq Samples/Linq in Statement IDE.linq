<Query Kind="Statements">
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

//The Statement ide
//this environment expects the use of C# statement grammar
//the results of a query is NOT automatically displayed as is
//		the Expression environment
//to display the results you need to .Dump() the variable
//		holding the data result
//IMPORTANT!! .Dump() is a Linqpad Method. It is NOT a C# method
//Within the Statement environment one can run ALL the queries 
//		in one execution
var qsyntaxlist = from arowoncollection in Albums
					select arowoncollection;
//qsyntaxlist.Dump();

var msyntaxlist = Albums
   			.Select (arowoncollection => arowoncollection)
			.Dump();
//msyntaxlist.Dump();

var QueenAlbums = Albums
	.Where(a => a.Artist.Name.Contains("Queen"))
	//.Dump()
	;