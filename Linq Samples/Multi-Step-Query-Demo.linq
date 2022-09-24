<Query Kind="Statements">
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

//Problem
//One needs to have processed information from a collection
//   to use against the same collection

//Solution to this type of problem is to use multiple queries
//	the early quer(ies) will produced the needed information/criteria
//	to execute against the same collection in later quer(ies)
//basicly we need to do some pre-processing


//query one will generate data/information that will be used in the
// next query (two)

//Display the employees that have the most customers to support.
//Display the employee name and number of customers that employee supports.

//What is NOT wnat is a list of all employees sorted by number of customers supported.

//One could create a list of all employees, with the customer support count, ordered
//	decsending by support count. BUT, this is NOT was is request.

//What information do I need
// a) I need to know the maximum number of customers that an particular employee is supporting
// b) I need to take that piece of data and compare to all employees.

//a) get a list of employees and the count of the customers each supports
//b) from that list I can obtain the largest number
//c) using the number, review all the employees and their counts, reporting ONLY the busiest
//			employees

var PreprocessEmployeeList = Employees
								.Select(x => new
										{
											Name = x.FirstName + " " + x.LastName,
											CustomerCount = x.SupportRepCustomers.Count()
										})
								//.Dump()
								;
//var highcount = PreprocessEmployeeList
//				.Max(x => x.CustomerCount)
//				//.Dump()
//				;
//var BusyEmployees = PreprocessEmployeeList
//					.Where(x => x.CustomerCount == highcount)
//					.Dump()
//					;
					
var BusyEmployees = PreprocessEmployeeList
					.Where(x => x.CustomerCount == 
								PreprocessEmployeeList.Max(x => x.CustomerCount))
					.Dump()
					;










