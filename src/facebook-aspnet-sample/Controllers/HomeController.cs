using System.Web.Mvc;

namespace facebook_aspnet_sample.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        [FacebookAuthorize]
        public ActionResult FacebookAccessTokens()
        {
            return View();
        }
    }
}
