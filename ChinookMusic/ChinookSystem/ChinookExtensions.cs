
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using ChinookSystem.DAL;
using ChinookSystem.BLL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace ChinookSystem
{
    //your class needs to be public so it can be used outside
    //  of this project
    //this class also needs to be static
    public static class ChinookExtensions
    {
        //method name can be anything, it must match
        //  the match the builder.Services.xxxxx(options => ..
        //  statement in your Program.cs

        //the first parameter is the class that you are attempting
        //  to extend

        //the second parameter is the options value in your
        //  call statement
        //it is receiving the connectionstring for your application

        public static void ChinookSystemBackendDependencies(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> options
            )
        {
            //register the DbContext class with the service collection
            services.AddDbContext<ChinookContext>(options);

            //add any services that you create in the class library
            //  using .AddTrainsient<serviceclassname>(....);

            services.AddTransient<TrackServices>((serviceProvider) =>
            {
                //retrieve the registered DcContext done with
                //  AddDbContext
                var context = serviceProvider.GetRequiredService<ChinookContext>();
                return new TrackServices(context);
            });

            services.AddTransient<PlaylistTrackServices>((serviceProvider) =>
            {
                //retrieve the registered DcContext done with
                //  AddDbContext
                var context = serviceProvider.GetRequiredService<ChinookContext>();
                return new PlaylistTrackServices(context);
            });

            services.AddTransient<AboutServices>((serviceProvider) =>
            {
                //retrieve the registered DcContext done with
                //  AddDbContext
                var context = serviceProvider.GetRequiredService<ChinookContext>();
                return new AboutServices(context);
            });
        }
    }
}
