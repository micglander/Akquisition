using System.Web;
using System.Web.Mvc;

namespace Akquisition
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            // mein Zeug
            filters.Add(new LogonAuthorizeAttribute());
            filters.Add(new Security.RequireSecureConnectionFilter());
        }
    }
}
