using System;
using System.Data;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace Trevental.DAL.Location.DBContext
{
    public class LocationDbContext
    {
        public LocationDbContext()
        {
            Console.WriteLine($"Connection string {JsonConvert.SerializeObject(ServiceCollectionExtensions.ConnectionString)}");
        }
        private IDbConnection instance = null;
        public IDbConnection Context {
            get {
                if (instance == null)
                {
                    instance = new MySqlConnection(ServiceCollectionExtensions.ConnectionString);
                }
                return instance;
            }
        }
    }
}