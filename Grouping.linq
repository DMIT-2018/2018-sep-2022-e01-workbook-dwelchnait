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

//Grouping

//when your create a group it builds two (2) components
//a) Key component (deciding creteria value(s)) defining the group
//		reference this component using the groupname.Key[.propertyname]
//			1 value for key: groupname.Key
//			n values for key: groupname.Key.propertyname
// (property < - > field < - > attribute < - > value)
//b) data of the groupe (raw instances of the collection)

//ways to group
//a) by a single column (field, attribute, property)  groupname.Key
//b) by a set of columns (anonymous dataset) 		  groupname.Key.property
//c) by use an entity (entity name/navproperty)		  groupname.Key.property

//concept processing
//start with a "pile" of data (original collection prior to grouping)
//speccify the grouping property(ies)
//result of the group operation will be to "place the data into smaller piles"
//	the piles are dependant on the grouping property(ies) value(s)
//	the grouping property(ies) become the Key
//	the individual instances are the data in the smaller piles
//	the entire individual instance of the original collection is place in the smaller pile
//manipulate each of the "smaller piles" using your linq commands

//grouping is different then Ordering
//Ordering is the final re-sequencing of a collection for display
//grouping re-organizes a collectiion into separate, usually smaller
//		collections for further processing (ie aggregates)

//grouping is an excellent way to organize your data especially if
//		you need to process data on a property that is "NOT" a relative key
//		such as a foreign key which forms a "natural" group using the
//		navigational properties

//Display albums by ReleaseYear
//	this request does NOT need grouping
//	this request is an ordering of output : OrderBy
//	this ordering affect only display
Albums
	.OrderBy(a => a.ReleaseYear)

//Display albums grouped by ReleaseYear
//	explicit request to breakup the display into desired "piles"
Albums
	.GroupBy(a => a.ReleaseYear)

//processing on the groups created by the Group command

//Display the number of albums produced each year
//list only the years which have more that 10 albums
Albums
	.GroupBy(a => a.ReleaseYear)
	.Where(egP => egP.Count() >10)  //filtering against each group pile
	.Select(eachgroupPile => new
		{
			Year = eachgroupPile.Key,
			NumOfAlbums = eachgroupPile.Count()
		})
	//.Where(x => x.NumOfAlbums >10) //filtering against the oupt of the .Select() command

//use a multiple ser of properties to form the group
// included a nested query to report on the small pile group

//Dsplay albums grouped by ReleaseLabel, ReleaseYear. Display the 
//ReleaseYear and number of albums. List only the years with 3 or
// more albums released. For each album display the title, artist, number of tracks on the
// album and release year

Albums
	.GroupBy(a => new {a.ReleaseLabel, a.ReleaseYear})
	.Where(egP => egP.Count() > 2)  //filtering against each group pile
	.Select(eachgroupPile => new
		{
			Label = eachgroupPile.Key.ReleaseLabel,
			Year = eachgroupPile.Key.ReleaseYear,
			NumOfAlbums = eachgroupPile.Count(),
			AlbumItems = eachgroupPile
							.Select(egPInstance => new
								{
									title = egPInstance.Title,
									artist = egPInstance.Artist.Name,
									trackcount = egPInstance.Tracks
													.Select(x => x).Count(),
									YearOfAlbum = egPInstance.ReleaseYear
								})
		})











