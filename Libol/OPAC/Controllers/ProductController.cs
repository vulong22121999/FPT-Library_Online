using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OPAC.Models;

namespace OPAC.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        [Route("Products")]
        public ActionResult ViewProductPage()
        {
            using (var dbContext = new OpacEntities())
            {
                ViewBag.Product = dbContext.NOTICE_STORE.ToList().FirstOrDefault(t => t.TypeID == 5);
            }

            return View();
        }
    }
}