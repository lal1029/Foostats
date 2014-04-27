using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Foostats2.Filters
{
    public class PopulateFoostatsViewContext : ActionFilterAttribute
    {
        public static string RootAvatarFolder = "~/Images/users/";
        public static List<string> ImageFormats = new List<string>() { "jpg", "png", "gif" };

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.User != null)
            {
                FoostatsViewContext foostatsViewContext = new FoostatsViewContext()
                {
                    UsersAvatar = null
                };

                var fullName = filterContext.RequestContext.HttpContext.User.Identity.Name.Split('\\');
                var domain = fullName[0];
                var alias = fullName[1];

                foreach (var format in ImageFormats)
                {
                    var stringPath = String.Format(
                                            "{0}{1}.{2}",
                                            RootAvatarFolder,
                                            alias,
                                            format);
                    string filePath = filterContext.RequestContext.HttpContext.Request.MapPath(stringPath);
                    if (System.IO.File.Exists(filePath))
                    {
                        foostatsViewContext.UsersAvatar = stringPath;
                        break;
                    }
                }

                filterContext.Controller.ViewBag.FoostatsViewContext = foostatsViewContext;
            }
            
        }
    }
}