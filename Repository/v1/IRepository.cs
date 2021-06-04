using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Options;
using Trevental.DAL.Location.DBContext;
namespace Trevental.DAL.Repository
{
    public interface IRepository<TEntity> where TEntity : class, new()
    {
        IEnumerable<TEntity> GetAll();

   
    }


    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
         

        public IEnumerable<TEntity> GetAll()
        {
             
           
                throw new Exception($"Couldn't retrieve entities: ");
             
        }

       
    }
}