using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Configuration;
using System.Net;
using System.Security.Principal;


namespace Akquisition
{
    public sealed class MyReportServerConnection : Microsoft.Reporting.WebForms.IReportServerConnection2
    {
        public WindowsIdentity ImpersonationUser
        {
            get
            {
                // Use the default Windows user.  Credentials will be
                // provided by the NetworkCredentials property.
                return null;
            }
        }

        public ICredentials NetworkCredentials
        {
            get
            {
                // Read the user information from the web.config file.  
                // By reading the information on demand instead of 
                // storing it, the credentials will not be stored in 
                // session, reducing the vulnerable surface area to the
                // web.config file, which can be secured with an ACL.

                // User name
                string userName =
                    ConfigurationManager.AppSettings
                        ["MyReportViewerUser"];

                if (string.IsNullOrEmpty(userName))
                    throw new Exception(
                        "Missing user name from Web.config file");

                // Password
                string password =
                    ConfigurationManager.AppSettings
                        ["MyReportViewerPassword"];

                if (string.IsNullOrEmpty(password))
                    throw new Exception(
                        "Missing password from Web.config file");

                // Domain
                string domain =
                    ConfigurationManager.AppSettings
                        ["MyReportViewerDomain"];

                if (string.IsNullOrEmpty(domain))
                    throw new Exception(
                        "Missing domain from Web.config file");

                return new NetworkCredential(userName, password, domain);
            }
        }

        public bool GetFormsCredentials(out Cookie authCookie,
                    out string userName, out string password,
                    out string authority)
        {
            authCookie = null;
            userName = null;
            password = null;
            authority = null;

            // Not using form credentials
            return false;
        }

        public Uri ReportServerUrl
        {
            get
            {
                string url =
                    ConfigurationManager.AppSettings[
                        "MyReportServerUrl"];

                if (string.IsNullOrEmpty(url))
                    throw new Exception(
                        "Missing url from the Web.config file");

                return new Uri(url);
            }
        }

        public int Timeout
        {
            get
            {
                return 60000; // 60 seconds
            }
        }

        public IEnumerable<Cookie> Cookies
        {
            get
            {
                // No custom cookies
                return null;
            }
        }

        public IEnumerable<string> Headers
        {
            get
            {
                // No custom headers
                return null;
            }
        }
    }
}