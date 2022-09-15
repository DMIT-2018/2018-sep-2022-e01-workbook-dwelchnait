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
	
	//Nested queries
	//sometimes referred to as subqueries
	//
	//simply put: it is a query within a query [....]
	
	//List all sales support employees showing their
	//	fullname (last, first), title and phone
	//For each employee, show a list of customers they support.
	//	Show the customer fullname (last, first), city and State
	
	//employee 1, title, phone
	//	customer 2000, city, state
	//	customer 2109, city, state
	//  customer 5000, city, state
	//employee 2, title, phone
	//	customer 301, city, state
	
	//there appears to be 2 separate lists that need to be
	//	within one final dataset collection
	// list of employees
	// list of employee customers
	//concern: the lists are intermixed!!!
	
	//C# point of view in a class definition
	//first: this is a composite class
	//   the class is describing an employee
	//	 each instance of the employee will have a list of employee customers
	
	//class EmployeeList
	//	fullname (property)
	//	title (property)
	//	phone (property)
	//  collection of customers (property: List<T>)
	
	//class CustomerList
	//  fullname (property)
	//  city (property)
	//  state (property)
	
	var results = Employees
				.Where(e => e.Title.Contains("Sales Support"))
				.Select(e => new EmployeeItem
				{
					FullName = e.LastName + ", " + e.FirstName,
					Title = e.Title,
					Phone = e.Phone,
					CustomerList = e.SupportRepCustomers
									.Select(c => new CustomerItem
									{
										FullName = c.LastName + ", "
													+ c.FirstName,
										City = c.City,
										State = c.State
									}
									)
				}
				);
	results.Dump();
}

public class CustomerItem
{
	public string FullName {get;set;}
	public string City {get;set;}
	public string State {get;set;}
}

public class EmployeeItem
{
	public string FullName {get;set;}
	public string Title {get;set;}
	public string Phone {get;set;}
	public IEnumerable<CustomerItem> CustomerList {get;set;}
}









