using HtmlAgilityPack;
using SearchEngineTask.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace SearchEngineTask.SearchEngine
{
    public class GoogleSearchEngine : ISearchEngine
    {
        private string _searchEngineUrl;

        public GoogleSearchEngine()
        {
            _searchEngineUrl = "https://www.google.com/search?&q=";
        }

        public ResponseModel GetResposeResult(string searchText)
        {
            var returnModel = new ResponseModel();

            // Start timer
            Stopwatch sw = Stopwatch.StartNew();
            var result = new HtmlWeb().Load(_searchEngineUrl + WebUtility.UrlEncode(searchText));
            sw.Stop();

            // Get descriptons from results
            var descriptionXPath = result.DocumentNode.SelectNodes("//html//body//div[@class='g']//span[@class='st']");
            var descriptions = descriptionXPath == null
                             ? new List<string> { "" }
                             : descriptionXPath.Select(x => WebUtility.HtmlDecode(x.InnerText))
                             .Take(10)
                             .ToList();

            // Get Title from results
            var titleXPath = result.DocumentNode.SelectNodes("//html//body//div[@class='g']//h3");
            var titles = titleXPath == null
                             ? new List<string> { "" }
                             : titleXPath.Select(x => WebUtility.HtmlDecode(x.InnerText))
                             .Take(10)
                             .ToList();

            // Get Link from results
            var linkXPath = result.DocumentNode.SelectNodes("//html//body//div[@class='g']//a[not(@id) and not(@class)]");
            var links = linkXPath == null
                              ? new List<string> { "" }
                              : linkXPath.Select(x => x.Attributes["href"].Value)
                              .Take(10)
                              .ToList();

            returnModel.Responses = links.Zip(
                titles.Zip(descriptions, Tuple.Create), (link, tuple) => new ResultModel(link, tuple.Item1, tuple.Item2)
                ).ToList();

            returnModel.ResponseDuration = sw.Elapsed;
            returnModel.SearchEngineName = "Google";
            return returnModel;
        }
    }
}