
namespace facebook_aspnet_sample.Controllers
{
    using System.Web.Mvc;
    using Facebook;

    public class HomeController : Controller
    {
        private readonly FacebookClient _fb;

        public HomeController()
            : this(new FacebookClient())
        {
        }

        public HomeController(FacebookClient fb)
        {
            _fb = fb;
        }

        public ActionResult Index()
        {
            return View();
        }

        [FacebookAuthorize]
        public ActionResult FacebookAccessTokens()
        {
            try
            {
                ViewBag.access_token = _fb.AccessToken;

                dynamic accounts = _fb.Get("me/accounts");
                ViewBag.accounts = accounts;
            }
            catch (FacebookOAuthException)
            {
                // log exception
                return new HttpUnauthorizedResult();
            }

            return View();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // it would be better to set the _fb.AccessToken using your favoirte IoC
            _fb.AccessToken = Session["fb_access_token"] as string;
        }
    }
}
