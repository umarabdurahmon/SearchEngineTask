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
            ViewBag.SearchText = SearchText; 
            foreach (string key in Request.Cookies.AllKeys)
            {
                HttpCookie c = Request.Cookies[key];
                c.Expires = DateTime.Now.AddMonths(-1);
                Response.AppendCookie(c);
            }
            IEnumerable<ISearchEngine> engine = new List<ISearchEngine>
            {
                //new BingSearchEngine() is added in try catch block because it uses async method,
                new GoogleSearchEngine(),
                new YandexSearchEngine()
            };

            var model = new ResponseModel();

            // Cheking if captcha exist
            try
            {
                var bing = await new BingSearchEngine().GetResposeResultAsync(SearchText);
                var list = engine.Select(p => p.GetResposeResult(SearchText)).ToList();
                list.Add(bing);

                list = list.Where(item => item.Responses.Count > 1).ToList();
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
            //request.Cookies = new Leaf.xNet.CookieStorage();

            request.UserAgent = Leaf.xNet.Http.ChromeUserAgent();
            request.AddHeader("key", WebUtility.UrlEncode(captchaModel.key));
            request.AddHeader("retpath", WebUtility.UrlEncode(captchaModel.retkey));
            request.AddHeader("rep", WebUtility.UrlEncode(captchaModel.rep));
            //request.AddHeader("Host","yandex.com");

            try
            {
                Leaf.xNet.HttpResponse response = request.Get($"http://yandex.com/xcheckcaptcha?key={captchaModel.key}&rep={captchaModel.rep}"); ; ;
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { SearchText = captchaModel.searchText });
            }
            return RedirectToAction("Index", new { SearchText = captchaModel.searchText });
        }

        /// <summary>
        /// All search requests
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchHistory()
        {
            return View(_dbService._database.Searches.OrderByDescending(x => x.SearchId));
        }

        /// <summary>
        /// Search results by search id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchResults(int id)
        {
            ViewBag.Seachtext = _dbService._database.Searches
                .Where(x => x.SearchId == id)
                .FirstOrDefault()
                .SearchText;
            return View(_dbService.GetSearchResults(id));
        }

        /// <summary>
        /// Json method: returns last 10 search request
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetHistory()
        {
            return Json(new { list = _dbService._database.Searches.OrderByDescending(x => x.SearchId).Take(10) }, JsonRequestBehavior.AllowGet);
        }

    }
}