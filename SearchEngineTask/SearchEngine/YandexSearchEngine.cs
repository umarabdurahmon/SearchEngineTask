using HtmlAgilityPack;
using SearchEngineTask.CustomExceptions;
using SearchEngineTask.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SearchEngineTask.SearchEngine
{
    public class YandexSearchEngine : ISearchEngine
    {
        private string _searchEngineUrl;

        public YandexSearchEngine()
        {
            _searchEngineUrl = "https://yandex.uz/search/?text=";
        }

        public ResponseModel GetResposeResult(string searchText)
        {
            var returnModel = new ResponseModel();

            // Start timer
            Stopwatch sw = Stopwatch.StartNew();            
            var web = new HtmlWeb();
            web.UseCookies = true;
            var result = web.Load(_searchEngineUrl + WebUtility.UrlEncode(searchText));

            sw.Stop();

            /// Match captcha if exist
            var captcha = result.DocumentNode.SelectNodes("//div[@class='captcha__image']//img");

            if (false /*captcha != null* captcha currently dont work*/)
            {
                //Get captcha parametrs from html
                HtmlNodeCollection inputs = captcha[0].SelectNodes("//input");
                string key_captcha = inputs[0].GetAttributeValue("value", "false").Replace("&amp;amp", "");
                string return_path_captcha = inputs[1].GetAttributeValue("value", "false").Replace("&amp;amp;", "&amp;");

                HtmlNode image = result.DocumentNode.SelectSingleNode("//div[@class='captcha__image']//img");
                string url_captcha = image.GetAttributeValue("src", "true");

                
                YandexCaptchaModel captchaModel = new YandexCaptchaModel()
                {
                    url_captcha = url_captcha,
                    key = key_captcha,
                    retkey = return_path_captcha,
                    searchText = searchText
                };

                throw new YandexCaptchaException(captchaModel);
            }

            // Get descriptons from results
            var descriptionXPath = result.DocumentNode.SelectNodes("//div[@class='serp-list']//div[@class='serp-item__text']");
            var descriptions = descriptionXPath == null
                             ? new List<string> { "" }
                             : descriptionXPath.Select(x => WebUtility.HtmlDecode(x.InnerText))
                             .Take(10)
                             .ToList();

            // Get Title from results
            var titleXPath = result.DocumentNode.SelectNodes("//div[@class='serp-list']//div[@class='serp-item']//h2[@class='serp-item__title']");
            var titles = titleXPath == null
                             ? new List<string> { "" }
                             : titleXPath.Select(x => WebUtility.HtmlDecode(x.InnerText))
                             .Take(10)
                             .ToList();

            // Get Link from results
            var linkXPath = result.DocumentNode.SelectNodes("//div[@class='serp-list']//span[@class='serp-url__item']//a[1]");
            var links = linkXPath == null
                              ? new List<string> { "" }
                              : linkXPath.Select(x => x.Attributes["href"].Value)
                              .Take(10)
                              .ToList();

            returnModel.Responses = links.Zip(
                titles.Zip(descriptions, Tuple.Create), (link, tuple) => new ResultModel(link, tuple.Item1, tuple.Item2)
                ).ToList();

            returnModel.ResponseDuration = sw.Elapsed;
            returnModel.SearchEngineName = "Yandex";
            
            return returnModel;
        }
    }
}
