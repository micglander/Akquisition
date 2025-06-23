using System;
using System.Web.Mvc;

namespace Akquisition.Security
{
    public class RequireSecureConnectionFilter : RequireHttpsAttribute
    {
        /// <summary>
        /// Erweiterung von RequireHttpsAttribute
        /// Lokale Anfragen erfordern KEIN https, zum Entwickeln!
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (filterContext.HttpContext.Request.IsLocal)
            {
                // when connection to the application is local, don't do any HTTPS stuff
                return;
            }

            base.OnAuthorization(filterContext);
        }
    }
}