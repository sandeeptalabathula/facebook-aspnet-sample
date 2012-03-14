using System.Web.Mvc;

namespace facebook_aspnet_sample
{
    public class FacebookAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            var accessToken = httpContext.Session["fb_access_token"] as string;
            if (string.IsNullOrWhiteSpace(accessToken))
                return false;
            return true;
        }
    }
}