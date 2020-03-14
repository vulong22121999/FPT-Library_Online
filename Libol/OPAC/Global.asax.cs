using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using OPAC.Models;

namespace OPAC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected void Application_Error()
        {
            var ex = Server.GetLastError();
            //log the error!
            log.Info("");
            log.Info("------------------------------------------------------------------------------------------------------");
            log.Error(ex.Message);
            log.Error(ex.StackTrace);
            log.Error(ex.TargetSite);

            string errorLogFilePath = Server.MapPath("~/ErrorLog/ErrorLog.txt");
            string errorContent = "Time: " + DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy") + Environment.NewLine +
                                  ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine +
                                  ex.TargetSite;
            System.IO.File.WriteAllText(errorLogFilePath, errorContent);
        }
    }
}
