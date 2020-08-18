using HtmlAgilityPack;
using SearchEngineTask.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SearchEngineTask.SearchEngine
{
    public class BingSearchEngine : ISearchEngine
    {
        private string _searchEngineUrl;

        public BingSearchEngine()
        {
            _searchEngineUrl = "https://www.bing.com/search?q=";
        }

        public async Task<ResponseModel> GetResposeResultAsync(string searchText)
        {
            var returnModel = new ResponseModel();

            // Start timer
            Stopwatch sw = Stopwatch.StartNew();
            /*
             * Bing does not return properly content with HtmlWeb class, However other engines returns properly.
             *
             * var web = new HtmlWeb();
            web.PreRequest = delegate (HttpWebRequest webReq)
            {
                webReq.Timeout = 140000; // number of milliseconds
                return true;
            };
            var result = web.Load(string.Format(_searchEngineUrl, WebUtility.UrlEncode(searchText)));
            */
            var httpclient = new HttpClient();
            var html = await httpclient.GetStringAsync(_searchEngineUrl + WebUtility.UrlEncode(searchText));
            var result = new HtmlDocument();
            result.LoadHtml(html);
            sw.Stop();

            // Get descriptons from results
            var descriptionXPath = result.DocumentNode.SelectNodes("//html//body//div//ol//li[@class='b_algo']//p");
            var descriptions = descriptionXPath == null
                             ? new List<string> { "" }
                             : descriptionXPath.Select(x => WebUtility.HtmlDecode(x.InnerText))
                             .Take(10)
                             .ToList();

            // Get Title from results
            var titleXPath = result.DocumentNode.SelectNodes("//html//body//div//ol//li[@class='b_algo']//h2");
            var titles = titleXPath == null
                             ? new List<string> { "" }
                             : titleXPath.Select(x => WebUtility.HtmlDecode(x.InnerText))
                             .Take(10)
                             .ToList();

            // Get Link from results
            var linkXPath = result.DocumentNode.SelectNodes("//html//body//div//ol//li[@class='b_algo']//h2//a");
            var links = linkXPath == null
                              ? new List<string> { "" }
                              : linkXPath.Select(x => x.Attributes["href"].Value)
                              .Take(10)
                              .ToList();

            returnModel.Responses = links.Zip(
                titles.Zip(descriptions, Tuple.Create), (link, tuple) => new ResultModel(link, tuple.Item1, tuple.Item2)
                ).ToList();

            returnModel.ResponseDuration = sw.Elapsed;
            returnModel.SearchEngineName = "Bing";
            return returnModel;
        }

        public ResponseModel GetResposeResult(string searchText)
        {
            return new ResponseModel();
        }
    }
}