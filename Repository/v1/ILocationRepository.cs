using System;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Linq;
using Dapper;
using Trevental.DAL.Location.DBContext;
using Trevental.DAL.Location;
using Trevental.Common.Sql;
using System.Collections.Generic;
using Trevental.Common.Services.Language;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Trevental.Common.Model.Location;
using Trevental.Common.Helper;
using Trevental.Common.Enums;
using Trevental.Common.Extensions;

namespace Trevental.DAL.Repository
{
     
    public interface ILocationRepository: IRepository<LocationModel>
    {   
        Task<IEnumerable<LocationModel>> Search(string query,CancellationToken cancellationToken);
        Task<IEnumerable<LocationModel>> SearchInIdList(string query,CancellationToken cancellationToken);
    }


    public class LocationRepository : Repository<LocationModel>, ILocationRepository
    {
        private readonly ILanguageService _languageService;
        
        //private IOptions<RequestLocalizationOptions> LocOptions;
        private readonly string currentLanguage;
        private bool isDefaultCutlture;
        private int LanguageId;
        public LocationRepository(ILanguageService languageService)
        {
            _languageService = languageService;
            this.currentLanguage= languageService.GetCurrentLanguage;

            this.LanguageId=_languageService.GetDefaultLanguageId;
            this.isDefaultCutlture=_languageService.GetLocalizationOptions.Value.DefaultRequestCulture.Culture.TwoLetterISOLanguageName.Equals(this.currentLanguage);
            
            //repo=_repo;
        }
         
                
        /// <param name="query">1,2,3,4,5,</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IEnumerable<LocationModel>> SearchInIdList(string query, CancellationToken cancellationToken){
            if (string.IsNullOrWhiteSpace(query)  ){
                return null;
            } 
            var flocation=SearchLocationByIdList(query);
            ToLocalize(ref flocation);
            return Task.FromResult(flocation);
        }
        public Task<IEnumerable<LocationModel>> Search(string query, CancellationToken cancellationToken)
        {
            query= Transactions.injection(query );
            if (string.IsNullOrWhiteSpace(query) || query.Length<2){
                return null;
            } 
            var flocation=SearchLocation(query);
            ToLocalize(ref flocation);
            return Task.FromResult(flocation);         
        }

         

        #region localize trasnform methods
         private void ToLocalize(ref IEnumerable<LocationModel> data){
            if(!isDefaultCutlture){
                data.ToList().ForEach(h=>{
                    #region 
                    var anycityName=h.LocalizeForMe(h.CityLocalesJson)?.Find(m=>m.LanguageId==this.LanguageId && m.LocaleKey== LocalizationEntityEnums.CityLocale.CityName.GetName());
                    h.CityName  =   anycityName?.LocaleValue ?? h.CityName;

                    var anyDistrict=h.LocalizeForMe(h.CityLocalesJson)?.Find(m=>m.LanguageId==this.LanguageId && m.LocaleKey== LocalizationEntityEnums.CityLocale.CityDistrict.GetName());
                    h.CityDistrict  =   anyDistrict?.LocaleValue ?? h.CityDistrict;
                    #endregion

                    #region 
                    var _countryName=h.LocalizeForMe(h.CountryLocalesJson)?.Find(m=>m.LanguageId==this.LanguageId && m.LocaleKey== LocalizationEntityEnums.CountryLocale.CountryName.GetName());
                    h.CountryName  =   _countryName?.LocaleValue ?? h.CountryName;

                    var _continent=h.LocalizeForMe(h.CountryLocalesJson)?.Find(m=>m.LanguageId==this.LanguageId && m.LocaleKey== LocalizationEntityEnums.CountryLocale.Continent.GetName());
                    h.Continent =   _continent?.LocaleValue ?? h.Continent;

                    var _region=h.LocalizeForMe(h.CountryLocalesJson)?.Find(m=>m.LanguageId==this.LanguageId && m.LocaleKey== LocalizationEntityEnums.CountryLocale.Region.GetName());
                    h.Region =   _region?.LocaleValue ?? h.Region;
                    
                    #endregion
                    
                });
            }
            //return data;
        }   
        
         
        #endregion

        #region  private methods
        private IEnumerable<LocationModel> SearchLocation(string query){
            
            query=query.ToLowerInvariant();

            string sqlQuery = @"SELECT 
            c1.cityID,c1.CityName,c1.CityCountryCode,c1.CityDistrict,c1.LocalesJson as CityLocalesJson,
            c2.CountryCode,c2.Continent,c2.CountryName,c2.LocalesJson as CountryLocalesJson
            FROM city c1 LEFT JOIN country c2 ON c2.CountryCode=c1.CityCountryCode 
            WHERE (
                (c1.CityName like @searchTerm OR LOWER(c1.LocalesJson) like @searchTerm) 
                    OR 
                (c2.CountryCode like @searchTerm OR c2.CountryName like @searchTerm OR c2.LocalName like @searchTerm OR c2.Region like @searchTerm OR LOWER(c2.LocalesJson) like @searchTerm)
                )
            Limit 0,100";
            var result= new MySqlRepository<LocationModel>(sqlQuery,new {searchTerm = query.ToEncodeForLikeQuery()},false).GetList;
            return result;
        }
        private IEnumerable<LocationModel> SearchLocationByIdList(string query){
            
            query=query.ToLowerInvariant();

            var cityIdList=new List<int>();
            var countryIdList=new List<string>();
            if(!string.IsNullOrWhiteSpace(query)){
                var locsep=query.Split(',').ToList();
                foreach (var item in locsep)
                {
                    if(Transactions.isNumeric(item)){
                        if(Convert.ToInt32(item)!=0){
                        cityIdList.Add(Convert.ToInt32(item));
                        }
                    }else if(!string.IsNullOrWhiteSpace(item)){
                        countryIdList.Add(item.ToString());
                    }
                }
            }

            string sqlQuery = @"SELECT 
            c1.cityID,c1.CityName,c1.CityCountryCode,c1.CityDistrict,c1.LocalesJson as CityLocalesJson,
            c2.CountryCode,c2.Continent,c2.CountryName,c2.LocalesJson as CountryLocalesJson
            FROM city c1 LEFT JOIN country c2 ON c2.CountryCode=c1.CityCountryCode 
            WHERE (
                (FIND_IN_SET(c1.CityID,@cityIdList)) 
                    OR 
                (FIND_IN_SET(c1.CityCountryCode,@countryCodeList))
                )
            Limit 0,100";
            var result= new MySqlRepository<LocationModel>(sqlQuery,new {
                cityIdList = string.Concat(string.Join(",",cityIdList)),
                countryCodeList=string.Concat(string.Join(",",countryIdList))
                },false).GetList;
            return result;
        }
        
       
        #endregion
    
    }
}