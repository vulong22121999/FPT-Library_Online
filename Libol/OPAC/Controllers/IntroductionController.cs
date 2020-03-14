using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OPAC.Models;

namespace OPAC.Controllers
{
    public class IntroductionController : Controller
    {
        // GET: Introduction
        [Route("Introductions")]
        public ActionResult ViewIntroductionPage()
        {
            using (var dbContext = new OpacEntities())
            {
                ViewBag.Intro = dbContext.NOTICE_STORE.ToList().FirstOrDefault(t => t.TypeID == 4);
            }

            return View();
        }
    }
}