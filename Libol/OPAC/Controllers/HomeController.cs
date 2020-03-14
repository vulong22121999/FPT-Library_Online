using OPAC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OPAC.Models.SupportClass;
using System.Data.Entity;
using System.Globalization;
using System.IO;

namespace OPAC.Controllers
{
    public class HomeController : Controller
    {
        private readonly OpacEntities _dbContext = new OpacEntities();

        // GET: Home
        public ActionResult Home()
        {
            if (TempData["Access"] == null)
            {
                CountVisitors();
            }

            var info = new DirectoryInfo(Server.MapPath("~/AllContent/home-images"));
            ViewBag.ImageHome = info.GetFiles().OrderByDescending(t => t.CreationTime).ToList();

            var model = new OptionModel
            {
                Option = OptionList()
            };

            //Get newest book by library
            var topNewBookFromHoaLac = _dbContext.SP_OPAC_GET_NEW_ITEMS(true, 0, 10, 81).ToList();
            var topNewBookFromDetect = _dbContext.SP_OPAC_GET_NEW_ITEMS(true, 0, 10, 20).ToList();
            var topNewBookFromFSBHaNoi = _dbContext.SP_OPAC_GET_NEW_ITEMS(true, 0, 10, 111).ToList();
            var topNewBookFromDaNang = _dbContext.SP_OPAC_GET_NEW_ITEMS(true, 0, 10, 24).ToList();
            var mostUsedBook = _dbContext.FPT_SP_OPAC_GET_MOST_USE_ITEMS().ToList();
            //DateTime for counter
            var today = DateTime.Today.ToString("dd/MM/yyyy");
            var yesterday =
                new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(-1).Day)
                    .ToString("dd/MM/yyyy");
            var thisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("dd/MM/yyyy");
            var lastMonth = DateTime.Now.Month == 1
                ? new DateTime(DateTime.Now.AddYears(-1).Year, 12, 1).ToString("dd/MM/yyyy")
                : new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 1).ToString("dd/MM/yyyy");

            foreach (var item in topNewBookFromHoaLac)
            {
                item.Content = RemoveSpecialCharacter(item.Content);
            }

            foreach (var item in topNewBookFromDetect)
            {
                item.Content = RemoveSpecialCharacter(item.Content);
            }

            foreach (var item in topNewBookFromFSBHaNoi)
            {
                item.Content = RemoveSpecialCharacter(item.Content);
            }

            foreach (var item in topNewBookFromDaNang)
            {
                item.Content = RemoveSpecialCharacter(item.Content);
            }

            foreach (var item in mostUsedBook)
            {
                item.Content = RemoveSpecialCharacter(item.Content);
            }

            //Pass newest books to view
            ViewBag.TopNewBookFromHoaLac = topNewBookFromHoaLac;
            ViewBag.TopNewBookFromDetect = topNewBookFromDetect;
            ViewBag.TopNewBookFromFSBHaNoi = topNewBookFromFSBHaNoi;
            ViewBag.TopNewBookFromDaNang = topNewBookFromDaNang;
            ViewBag.MostUsedBook = mostUsedBook;
            CounterStatistics counter = GetCounterStatistics();
            ViewBag.CounterStatistics = counter;
            ViewBag.Total = counter.GetTotal();

            //TypeID = 1 : News, 2 : Notification
            ViewBag.News = _dbContext.NOTICE_STORE.Where(t => t.TypeID == 1).OrderByDescending(t => t.CreateTime).Take(4).ToList();
            ViewBag.FullNews = _dbContext.NOTICE_STORE.Where(t => t.TypeID == 1).OrderByDescending(t => t.CreateTime).ToList();
            ViewBag.Notification = _dbContext.NOTICE_STORE.Where(t => t.TypeID == 2).OrderByDescending(t => t.CreateTime)
                .Take(4).ToList();
            ViewBag.FullNotification = _dbContext.NOTICE_STORE.Where(t => t.TypeID == 2).OrderByDescending(t => t.CreateTime).ToList();

            //Pass date for counter to view
            ViewBag.Today = today;
            ViewBag.Yesterday = yesterday;
            ViewBag.ThisMonth = thisMonth;
            ViewBag.LastMonth = lastMonth;

            TempData["CountResultList"] = 0;
            TempData["Flag"] = null;

            return View(model);
        }

        //List option for searching book
        private List<SelectListItem> OptionList()
        {
            List<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem {Text = "Mọi trường", Value = "0"},
                new SelectListItem {Text = "Nhan đề", Value = "1"},
                new SelectListItem {Text = "Tác giả", Value = "2"},
                new SelectListItem {Text = "Nhà xuất bản", Value = "3"},
                new SelectListItem {Text = "Chỉ số DDC", Value = "4"},
                new SelectListItem {Text = "Ngôn ngữ", Value = "5"},
                new SelectListItem {Text = "Từ khóa", Value = "6"},
                new SelectListItem {Text = "Môn học", Value = "7"}
            };
            return items;
        }

        private string RemoveSpecialCharacter(string input)
        {
            return input.Replace("$a", "").Replace("$b", " ")
                .Replace("$c", " ").Replace("$e", " ").Replace("$n", " ")
                .Replace("$p", " ");
        }

        //Get data of counter visitor
        private CounterStatistics GetCounterStatistics()
        {
            var statisticsList = _dbContext.SYS_COUNTER.ToList();
            int countToday = 0;
            int countYesterday = 0;
            int countThisWeek = 0;
            int countLastWeek = 0;
            int countThisMonth = 0;
            int countLastMonth = 0;
            DateTime today = DateTime.Now.Date;
            DateTime yesterday = today.AddDays(-1);
            DateTime startCurrentWeekDate = DateTime.Today.AddDays(-1 * (int) DateTime.Today.DayOfWeek).AddDays(1);
            DateTime endCurrentWeekDate = startCurrentWeekDate.AddDays(7).AddSeconds(-1);
            DateTime startLastWeekDate = startCurrentWeekDate.AddDays(-7);
            DateTime endLastWeekDate = startLastWeekDate.AddDays(7).AddSeconds(-1);
            DateTime startCurrentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endCurrentMonth = startCurrentMonth.AddMonths(1).AddSeconds(-1);
            DateTime startLastMonth;
            DateTime endLastMonth;
            if (DateTime.Now.Month == 1)
            {
                startLastMonth = new DateTime(DateTime.Now.AddYears(-1).Year, 12, 1);
                endLastMonth = new DateTime(DateTime.Now.AddYears(-1).Year, DateTime.Now.Month, 1);
            }
            else
            {
                startLastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 1);
                endLastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddSeconds(-1);
            }

            foreach (var item in statisticsList)
            {
                if (item.AccessedDate.Date == today.Date)
                {
                    countToday += Convert.ToInt32(item.AccessedTime);
                }

                if (item.AccessedDate.Date == yesterday.Date)
                {
                    countYesterday += Convert.ToInt32(item.AccessedTime);
                }

                if (BetweenTime(item.AccessedDate, startCurrentWeekDate, endCurrentWeekDate))
                {
                    countThisWeek += Convert.ToInt32(item.AccessedTime);
                }

                if (BetweenTime(item.AccessedDate, startLastWeekDate, endLastWeekDate))
                {
                    countLastWeek += Convert.ToInt32(item.AccessedTime);
                }

                if (BetweenTime(item.AccessedDate, startCurrentMonth, endCurrentMonth))
                {
                    countThisMonth += Convert.ToInt32(item.AccessedTime);
                }

                if (BetweenTime(item.AccessedDate, startLastMonth, endLastMonth))
                {
                    countLastMonth += Convert.ToInt32(item.AccessedTime);
                }
            }

            CounterStatistics counterStatistics = new CounterStatistics()
            {
                Today = countToday,
                Yesterday = countYesterday,
                ThisWeek = countThisWeek,
                LastWeek = countLastWeek,
                ThisMonth = countThisMonth,
                LastMonth = countLastMonth
            };

            return counterStatistics;
        }

        //check condition for datetime
        private static bool BetweenTime(DateTime input, DateTime startDate, DateTime endDate)
        {
            return (input >= startDate && input <= endDate);
        }

        //Save statistic number of visitors into database
        private void CountVisitors()
        {
            //Count 1 for each time user accessing to website
            var listUserAddress = _dbContext.SYS_COUNTER.ToList();

            foreach (var item in listUserAddress)
            {
                if (item.IPAddress.Equals(Request.UserHostAddress) && item.AccessedDate.Date == DateTime.Now.Date)
                {
                    item.AccessedTime += 1;
                    _dbContext.Entry(item).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    return;
                }

                if (item.IPAddress.Equals("::1"))
                {
                    item.AccessedTime = 0;
                    _dbContext.Entry(item).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                }
            }

            // if ip address and datetime are not existed in database
            SYS_COUNTER counter = new SYS_COUNTER()
            {
                IPAddress = Request.UserHostAddress,
                AccessedDate = DateTime.Now,
                AccessedTime = 1
            };

            _dbContext.SYS_COUNTER.Add(counter);
            _dbContext.SaveChanges();
        }
    }
}