
namespace facebook_aspnet_sample.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web.Mvc;
    using Facebook;

    public class AccountController : Controller
    {
        private const string clientId = ""
        private const string clientSecret = ""
        private const string scope = "user_about_me,publish_stream,manage_pages";
        private const string redirectUri = "http://localhost:22466/Account/FacebookCallback";

        private readonly FacebookClient _fb;

        public AccountController()
            : this(new FacebookClient())
        {
        }

        public AccountController(FacebookClient fb)
        {
            _fb = fb;
        }

        public ActionResult LogOn(string returnUrl)
        {
            var csrfToken = Guid.NewGuid().ToString();
            Session["fb_csrf_token"] = csrfToken;

            var state = Convert.ToBase64String(Encoding.UTF8.GetBytes(_fb.SerializeJson(new { returnUrl = returnUrl, csrf = csrfToken })));

            var fbLoginUrl = _fb.GetLoginUrl(
                new
                    {
                        client_id = clientId,
                        client_secret = clientSecret,
                        redirect_uri = redirectUri,
                        response_type = "code",
                        scope = scope,
                        state = state
                    });

            return Redirect(fbLoginUrl.AbsoluteUri);
        }

        public ActionResult FacebookCallback(string code, string state)
        {
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(state))
                return RedirectToAction("Index", "Home");

            // first validate the csrf token
            dynamic decodedState;
            try
            {
                decodedState = _fb.DeserializeJson(Encoding.UTF8.GetString(Convert.FromBase64String(state)), null);
                var exepectedCsrfToken = Session["fb_csrf_token"] as string;
                if (!(decodedState is IDictionary<string, object>) || !decodedState.ContainsKey("csrf") || string.IsNullOrWhiteSpace(exepectedCsrfToken) || exepectedCsrfToken != decodedState.csrf)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                // log exception
                return RedirectToAction("Index", "Home");
            }

            try
            {
                dynamic result = _fb.Post("oauth/access_token",
                                          new
                                              {
                                                  client_id = clientId,
                                                  client_secret = clientSecret,
                                                  redirect_uri = redirectUri,
                                                  code = code
                                              });

                Session["fb_access_token"] = result.access_token;

                if (result.ContainsKey("expires"))
                    Session["fb_expires_in"] = DateTime.Now.AddSeconds(result.expires);

                if (decodedState.ContainsKey("returnUrl"))
                {
                    if (Url.IsLocalUrl(decodedState.returnUrl))
                        return Redirect(decodedState.returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                // log exception
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
