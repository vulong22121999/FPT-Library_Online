using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OPAC.Models;

namespace OPAC.Controllers
{
    public class ServiceController : Controller
    {
        // GET: Service
        [Route("Services")]
        public ActionResult ViewServicePage()
        {
            using (var dbContext = new OpacEntities())
            {
                ViewBag.Service = dbContext.NOTICE_STORE.ToList().FirstOrDefault(t => t.TypeID == 6);
            }

            return View();
        }
    }
}