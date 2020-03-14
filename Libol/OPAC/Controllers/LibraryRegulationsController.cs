using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OPAC.Models;

namespace OPAC.Controllers
{
    public class LibraryRegulationsController : Controller
    {
        // GET: LibraryRegulations
        [Route("LibraryRegulations")]
        public ActionResult ViewLibraryRegulation()
        {
            using (var dbContext = new OpacEntities())
            {
                ViewBag.Regulation = dbContext.NOTICE_STORE.ToList().FirstOrDefault(t => t.TypeID == 3);
            }

            return View();
        }
    }
}