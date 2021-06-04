using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Trevental.DAL.Location.DBContext;
using MediatR;
using Trevental.DAL.Repository;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using Trevental.DAL.Location.Service.Query;
using Trevental.Common;
using Microsoft.AspNetCore.Builder;

namespace Trevental.DAL.Location
{
    public static class ServiceCollectionExtensions
    {
        public static string ConnectionString ; 
        
        public static void AddDALServices(this IServiceCollection services,
        string connString
        //,IOptions<RequestLocalizationOptions> options
        )
        {
            ConnectionString=connString;
            //LocalizationOptions=options;

            services.AddOptions();
            services.AddMediatR(typeof(LocationDbContext));
            
            //services.AddScoped(typeof(ISqlRepository<>),typeof(MySqlRepository<>));
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<LocationDbContext>();
            services.AddTransient<GetLocationByIdQuery>();
            services.AddTransient<ILocationRepository, LocationRepository>();
            //services.AddTransient<IRepository, Repository>();

            services.AddServicesToCommonLibrary(connString);
        }
    }
}