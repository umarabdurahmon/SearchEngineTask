﻿using SearchEngineTask.Database.DbModels;
using SearchEngineTask.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchEngineTask.Database.Services
{
    public class SearchService
    {
        private SearchEngineDbContext _database;

        public SearchService()
        {
            _database = new SearchEngineDbContext();
        }

        public bool InsertSearchResults(ResponseModel responseModel, string SearchText)
        {
            try
            {
                var searchModel = new Search()
                {
                    SearchEngineName = responseModel.SearchEngineName,
                    SearchText = SearchText
                };

                var insertedModel = _database.Searches.Add(searchModel);

                foreach (var item in responseModel.Responses)
                {
                    _database.SearchesResults.Add(new SearchResult() 
                    {
                        Url = item.Link,
                        Title = item.Title,
                        Description = item.Description,
                        Search = searchModel,   
                        SearchId = insertedModel.SearchId
                    });
                }

                _database.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }

            return true;
        }

        public IEnumerable<Search> GetSearches()
        {
            return _database.Searches;
        }

        public IEnumerable<SearchResult> GetSearchResults(int id)
        {
            return _database.SearchesResults.Where(x => x.SearchId == id);
        }
    }
}