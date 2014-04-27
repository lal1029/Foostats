using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Foostats2.Utility
{
    public static class ImageHelper
    {
        private const string UserName = @"REDMOND\amayoub";
        private const string Password = "********"; // replace in production
        private const string TempPath = @"C:\tmp\";

        public const string RedmondTemplate = "http://who/Photos/{0}.jpg";

        private static Uri GetUrl(string domain, string alias){
            if(string.Compare(domain, "Redmond", true) == 0){
                return new Uri(string.Format(RedmondTemplate, alias));
            } else {
                return null;
            }
        }

        public static string GetImage(string domain, string alias)
        {
            var url = GetUrl(domain, alias);
            WebRequest req = WebRequest.Create(url);
            req.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
            var cred = new NetworkCredential()
            {
                UserName = UserName,
                Password = Password
            };

            WebResponse resp = req.GetResponse();
            var img = Image.FromStream(resp.GetResponseStream());
            var path = Path.Combine(TempPath, string.Format("{0}_{1}.jpg", domain, alias));
            img.Save(path);
            return path;
        }
    }
}