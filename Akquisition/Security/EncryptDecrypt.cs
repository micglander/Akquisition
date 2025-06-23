using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Configuration;
using System.Web.Configuration;
using System.Web.Security;

namespace Akquisition.Security
{
    public class EncryptDecrypt
    {
        public static string path = HttpContext.Current.Request.ApplicationPath;

        public static void EncryptConnectioString()
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration(path);
            ConfigurationSection section = config.GetSection("connectionStrings");
            if (!section.SectionInformation.IsProtected)
            {
                section.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
                config.Save();
            }
        }

        public static void DecryptConnectionString()
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration(path);
            ConfigurationSection section = config.GetSection("connectionStrings");
            if (section.SectionInformation.IsProtected)
            {
                section.SectionInformation.UnprotectSection();
                config.Save();
            }
        }
    }
}