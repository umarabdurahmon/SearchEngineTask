using SearchEngineTask.CustomExceptions;
using SearchEngineTask.Database;
using SearchEngineTask.Database.Services;
using SearchEngineTask.Models;
using SearchEngineTask.SearchEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SearchEngineTask.Controllers
{
    public class HomeController : Controller
    {
        SearchService _dbService = new SearchService();

        /// <summary>
        /// Get Method
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Index()
        {
            var model = new ResponseModel();
            return View(model);
        }

        /// <summary>
        /// Post: get string(search text) returns 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Index(string SearchText)
        {
            foreach (string key in Request.Cookies.AllKeys)
            {
                HttpCookie c = Request.Cookies[key];
                c.Expires = DateTime.Now.AddMonths(-1);
                Response.AppendCookie(c);
            }
            IEnumerable<ISearchEngine> engine = new List<ISearchEngine>
            {
                //new BingSearchEngine(),
                new GoogleSearchEngine(),
                new YandexSearchEngine()
            };

            var model = new ResponseModel();

            try
            {
                var bing = await new BingSearchEngine().GetResposeResultAsync(SearchText);
                var list = engine.Select(p => p.GetResposeResult(SearchText)).ToList();
                list.Add(bing);

                model = list.Where(item => item.ResponseDuration == (list.Min(x => x.ResponseDuration))).FirstOrDefault();

                _dbService.InsertSearchResults(model, SearchText);

                return View(model);
            }
            catch (YandexCaptchaException ex)
            {
                model.Captcha = ex.YandexCapcha;
                return View(model);
            }       
        }

        /// <summary>
        /// Verifing captcha (only for Yandex search engine)
        /// </summary>
        /// <param name="captchaModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult VerfiyCaptcha(YandexCaptchaModel captchaModel)
        {
            
            var request = new Leaf.xNet.HttpRequest();            
            request.Cookies = new Leaf.xNet.CookieStorage();

            request.UserAgent = Leaf.xNet.Http.ChromeUserAgent();
            request.AddHeader("key", WebUtility.UrlEncode(captchaModel.key));
            request.AddHeader("retpath", WebUtility.UrlEncode(captchaModel.retkey));
            request.AddHeader("rep", WebUtility.UrlEncode(captchaModel.rep));
            request.AddHeader("Host","yandex.uz");

            Leaf.xNet.HttpResponse response = request.Get(@"http://yandex.uz/checkcaptcha");
            return RedirectToAction("Index", new { SearchText = captchaModel.searchText });
        }

        [HttpGet]
        public ActionResult SearchHistory()
        {
            return View(_dbService.GetSearches());
        }

        [HttpGet]
        public ActionResult SearchResults(int id)
        {
            return View(_dbService.GetSearchResults(id));
        }



    }
}