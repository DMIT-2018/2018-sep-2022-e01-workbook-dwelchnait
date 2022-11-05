
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

#region Additional Namespaces
using ChinookSystem;
#endregion

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//supplied database connection due to the fact that
//  we created this web app to use Individual Accounts
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//add another GetConnectionString to reference our database
//  connectionString
var connectionStringChinook = 
    builder.Configuration.GetConnectionString("ChinookDB");

//given for the db connection to Defaultconnection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//code the dbconnection to the application DB context for Chinook
//the implementation of the connect AND registration of the
//  ChinookSystem services will be done in the ChinookSystem
//  class library
//so to accomplish this tak, we will be using an "extension method"
//the extension method will extend the IServiceCollection Class
//the extension method requires a parameter options.UseSqlServer(XXX)
//  where xxx is the connection string variable
builder.Services.ChinookSystemBackendDependencies(options =>
            options.UseSqlServer(connectionStringChinook));



builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
