using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OPAC.Models;
using OPAC.Models.SupportClass;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using OPAC.Dao;
using PagedList;

namespace OPAC.Controllers
{
    public class NotificationController : Controller
    {
        private readonly OpacEntities _dbContext = new OpacEntities();

        // GET: Notification
        public ActionResult CreateNotification()
        {
            if (Session["LibrarianName"] == null)
            {
                return RedirectToAction("LibrarianLogin", "Login");
            }

            var typeNoticeList = _dbContext.TYPE_NOTICE.ToList();
            typeNoticeList.Insert(0, new TYPE_NOTICE()
            {
                ID = 0,
                TypeNotice = "-------- Chọn mục --------"
            });
            var libraryList = _dbContext.HOLDING_LIBRARY.Where(e => e.Name != null).ToList();
            libraryList.Insert(0, new HOLDING_LIBRARY()
            {
                ID = 0,
                Code = "------ Chọn thư viện ------"
            });
            ViewBag.TypeNotice = typeNoticeList;
            ViewBag.Library = libraryList;
            TempData.Remove("SearchNoticeList");

            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult CreateNotification(Notice data)
        {
            if (data.TypeID == 0 && data.LibID == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn mục đăng bài và thư viện!";

                return RedirectToAction("CreateNotification");
            }

            if (data.TypeID == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn mục đăng bài!";

                return RedirectToAction("CreateNotification");
            }

            if (data.LibID == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn thư viện!";

                return RedirectToAction("CreateNotification");
            }

            if (string.IsNullOrWhiteSpace(data.Title))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập tiêu đề!";

                return RedirectToAction("CreateNotification");
            }

            if (string.IsNullOrWhiteSpace(data.Content))
            {
                TempData["ErrorMessage"] = "Vui lòng thêm nội dung bài viết!";

                return RedirectToAction("CreateNotification");
            }

            var isExisted = _dbContext.NOTICE_STORE.ToList().Any(t => t.TypeID == data.TypeID);

            if (isExisted && data.TypeID == 3 || isExisted && data.TypeID == 4 || isExisted && data.TypeID == 5 || 
                isExisted && data.TypeID == 6)
            {
                TempData["ErrorMessage"] = "Mục " + HomeDao.GetTypeNotice(data.TypeID) + " chỉ được đăng 1 bài viết!";
            
                return RedirectToAction("CreateNotification");
            }

            if (ModelState.IsValid)
            {
                NOTICE_STORE obj = new NOTICE_STORE
                {
                    Title = data.Title,
                    Content = data.Content,
                    CreateTime = DateTime.Now,
                    TypeID = data.TypeID,
                    LibID = data.LibID
                };
                _dbContext.NOTICE_STORE.Add(obj);
                _dbContext.SaveChanges();
            }

            TempData["SuccessfulMessage"] = "Đăng bài thành công!!";

            return RedirectToAction("NotificationHome", new { page = 1 });
        }

        [Route("DetailNotice")]
        public ActionResult DetailNotice(int id)
        {
            var notice = _dbContext.NOTICE_STORE.FirstOrDefault(t => t.ID == id);

            //Get 5 newest notification
            ViewBag.TopNotification = _dbContext.NOTICE_STORE.Where(t => t.TypeID == 2 && t.ID != id)
                .OrderByDescending(t => t.CreateTime).Take(5).ToList();
            //Get 5 newest news
            ViewBag.TopNews = _dbContext.NOTICE_STORE.Where(t => t.TypeID == 1 && t.ID != id)
                .OrderByDescending(t => t.CreateTime).Take(5).ToList();

            return View(notice);
        }

        public ActionResult NotificationHome(int page)
        {
            if (Session["LibrarianName"] == null)
            {
                return RedirectToAction("LibrarianLogin", "Login");
            }

            var noticeListFromDb = _dbContext.NOTICE_STORE.OrderByDescending(t => t.CreateTime).ToList();
            var noticeList = new List<Notification>();
            int size = 10;
            int count = 0;

            if (TempData.Peek("SearchNoticeList") != null)
            {
                noticeList = (List<Notification>) TempData.Peek("SearchNoticeList");
            }
            else
            {
                foreach (var item in noticeListFromDb)
                {
                    Notification notification = new Notification()
                    {
                        ID = item.ID,
                        Title = item.Title,
                        Content = item.Content,
                        CreateTime = item.CreateTime.ToString("HH:mm:ss dd/MM/yyyy"),
                        TypeName = HomeDao.GetTypeNotice(item.TypeID),
                        LibraryName = HomeDao.GetLibraryCode(item.LibID)
                    };

                    noticeList.Add(notification);
                }
            }

            foreach (var item in noticeList)
            {
                count++;
                item.Count = count;
            }

            ViewBag.TypeNoticeList = _dbContext.TYPE_NOTICE.ToList();
            ViewBag.LibraryList = _dbContext.HOLDING_LIBRARY.Where(t => t.Name != null).ToList();
            ViewBag.FirstIndex = noticeList.ToPagedList(page, size).FirstItemOnPage;
            ViewBag.LastIndex = noticeList.ToPagedList(page, size).LastItemOnPage;
            ViewBag.Total = noticeList.ToPagedList(page, size).TotalItemCount;

            return View(noticeList.ToPagedList(page, size));
        }

        public ActionResult EditNotification(int noticeId)
        {
            if (Session["LibrarianName"] == null)
            {
                return RedirectToAction("LibrarianLogin", "Login");
            }

            var typeNoticeList = _dbContext.TYPE_NOTICE.ToList();
            typeNoticeList.Insert(0, new TYPE_NOTICE()
            {
                ID = 0,
                TypeNotice = "-------- Chọn mục --------"
            });
            var libraryList = _dbContext.HOLDING_LIBRARY.Where(e => e.Name != null).ToList();
            libraryList.Insert(0, new HOLDING_LIBRARY()
            {
                ID = 0,
                Code = "------ Chọn thư viện ------"
            });
            ViewBag.TypeNotice = typeNoticeList;
            ViewBag.Library = libraryList;

            var noticeFromDb = _dbContext.NOTICE_STORE.FirstOrDefault(t => t.ID == noticeId);
            Notice notice = new Notice();

            if (noticeFromDb != null)
            {
                notice = new Notice()
                {
                    ID = noticeId,
                    Title = noticeFromDb.Title,
                    Content = noticeFromDb.Content,
                    CreateTime = noticeFromDb.CreateTime,
                    TypeID = noticeFromDb.TypeID,
                    LibID = noticeFromDb.LibID
                };
            }

            TempData.Remove("SearchNoticeList");

            return View(notice);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EditNotification(int ID, int TypeID, int LibID, string Title, string Content)
        {
            if (TypeID == 0 && LibID == 0)
            {
                TempData["ErrorUpdateMessage"] = "Vui lòng chọn mục đăng bài và thư viện!";

                return RedirectToAction("EditNotification", new { noticeId = ID });
            }

            if (TypeID == 0)
            {
                TempData["ErrorUpdateMessage"] = "Vui lòng chọn mục đăng bài!";

                return RedirectToAction("EditNotification", new { noticeId = ID });
            }

            if (LibID == 0)
            {
                TempData["ErrorUpdateMessage"] = "Vui lòng chọn thư viện!";

                return RedirectToAction("EditNotification", new { noticeId = ID });
            }

            if (string.IsNullOrEmpty(Title.Trim()))
            {
                TempData["ErrorUpdateMessage"] = "Vui lòng nhập tiêu đề!";

                return RedirectToAction("EditNotification", new { noticeId = ID });
            }

            if (string.IsNullOrEmpty(Content.Trim()))
            {
                TempData["ErrorUpdateMessage"] = "Vui lòng nhập nội dung bài viết!";

                return RedirectToAction("EditNotification", new { noticeId = ID });
            }

            if (ModelState.IsValid)
            {
                var notice = _dbContext.NOTICE_STORE.FirstOrDefault(t => t.ID == ID);

                if (notice != null)
                {
                    notice.Title = Title;
                    notice.Content = Content;
                    notice.TypeID = TypeID;
                    notice.LibID = LibID;
                    notice.CreateTime = DateTime.Now;
                }

                _dbContext.Entry(notice).State = EntityState.Modified;
                _dbContext.SaveChanges();
            }

            TempData["UpdateSuccessfulMessage"] = "Cập nhật bài viết thành công!!";
            TempData.Remove("SearchNoticeList");

            return RedirectToAction("NotificationHome", new {page = 1});
        }

        public ActionResult DeleteNotification(string strOrderID)
        {
            var temp = strOrderID.Trim().Split(' ');
            var noticeList = _dbContext.NOTICE_STORE.ToList();

            foreach (var item in temp)
            {
                var notice = noticeList.FirstOrDefault(t => t.ID == Convert.ToInt32(item));
                if (notice != null)
                {
                    _dbContext.Entry(notice).State = EntityState.Deleted;
                    _dbContext.SaveChanges();
                }
            }

            TempData.Remove("SearchNoticeList");

            return Json(string.Empty);
        }

        public ActionResult SelectDeletingNotification(string strOrderID)
        {
            var temp = strOrderID.Trim().Split(' ');
            var noticeList = _dbContext.NOTICE_STORE.ToList();

            foreach (var item in temp)
            {
                var notice = noticeList.FirstOrDefault(t => t.ID == Convert.ToInt32(item));
                if (notice != null)
                {
                    _dbContext.Entry(notice).State = EntityState.Deleted;
                    _dbContext.SaveChanges();
                }
            }

            return Json(string.Empty);
        }

        public ActionResult FullNotification(int page)
        {
            int size = 30;
            var noticeList = _dbContext.NOTICE_STORE.Where(t => t.TypeID == 2).OrderByDescending(t => t.CreateTime)
                .ToPagedList(page, size);
            ViewBag.CurrentPage = noticeList.PageNumber;
            ViewBag.TotalPage = noticeList.PageCount;

            return View(noticeList);
        }

        public ActionResult FullNews(int page)
        {
            int size = 30;
            var noticeList = _dbContext.NOTICE_STORE.Where(t => t.TypeID == 1).OrderByDescending(t => t.CreateTime)
                .ToPagedList(page, size);
            ViewBag.CurrentPage = noticeList.PageNumber;
            ViewBag.TotalPage = noticeList.PageCount;

            return View(noticeList);
        }

        public ActionResult NotificationByLibrary(int libId, int page)
        {
            int size = 30;
            var noticeList = _dbContext.NOTICE_STORE.Where(t => t.LibID == libId).OrderByDescending(t => t.CreateTime)
                .ToPagedList(page, size);
            ViewBag.LibraryID = libId;
            ViewBag.CurrentPage = noticeList.PageNumber;
            ViewBag.TotalPage = noticeList.PageCount;

            return View(noticeList);
        }

        [HttpPost]
        public ActionResult SearchNotification(string searchByTitle, int searchByType, int searchByLibrary, 
            string startDate, string endDate)
        {
            var noticeListFromDb = _dbContext.NOTICE_STORE.AsQueryable();
            var theStartTime = ConvertToDate(startDate);
            var theEndTime = ConvertToDate(endDate);
            //Item1: year, Item2: month, Item3: day
            var startDateTime = new DateTime(theStartTime.Item1, theStartTime.Item2, theStartTime.Item3);
            var endDateTime = new DateTime(theEndTime.Item1, theEndTime.Item2, theEndTime.Item3);
            string title = Regex.Replace(searchByTitle, @"\s+", " ").Trim();

            if (string.IsNullOrEmpty(title) && searchByLibrary == 0 && searchByType == 0
                && CheckNullDateTime(startDateTime) && CheckNullDateTime(endDateTime))
            {
                TempData["SearchNoticeList"] = null;
            }
            else
            {
                if (!string.IsNullOrEmpty(title))
                {
                    noticeListFromDb = noticeListFromDb.Where(t => t.Title.Contains(title) || t.Title.Equals(title));
                }

                if (searchByType != 0)
                {
                    noticeListFromDb = noticeListFromDb.Where(t => t.TypeID == searchByType);
                }

                if (searchByLibrary != 0)
                {
                    noticeListFromDb = noticeListFromDb.Where(t => t.LibID == searchByLibrary);
                }

                if (!CheckNullDateTime(startDateTime) && !CheckNullDateTime(endDateTime))
                {
                    noticeListFromDb = noticeListFromDb.Where(t => t.CreateTime >= startDateTime && t.CreateTime <= endDateTime);
                }

                if (!CheckNullDateTime(startDateTime))
                {
                    noticeListFromDb = noticeListFromDb.Where(t => t.CreateTime >= startDateTime);
                }

                if (!CheckNullDateTime(endDateTime))
                {
                    noticeListFromDb = noticeListFromDb.Where(t => t.CreateTime <= endDateTime);
                }

                var noticeStoreList = noticeListFromDb.OrderByDescending(t => t.CreateTime).ToList();
                var noticeList = (from t in noticeStoreList
                    select new Notification()
                    {
                        ID = t.ID,
                        Title = t.Title,
                        Content = t.Content,
                        CreateTime = t.CreateTime.ToString("HH:mm:ss dd/MM/yyyy"),
                        TypeName = HomeDao.GetTypeNotice(t.TypeID),
                        LibraryName = HomeDao.GetLibraryCode(t.LibID)
                    }).ToList();

                TempData["SearchNoticeList"] = noticeList;
            }

            return RedirectToAction("NotificationHome", new {page = 1});
        }

        public ActionResult ReturnNotificationHome()
        {
            TempData.Remove("SearchNoticeList");

            return RedirectToAction("NotificationHome", "Notification", new {page = 1});
        }

        public ActionResult CheckDuplicatedType(string typeId)
        {
            int noticeId = Convert.ToInt32(typeId);
            var isExisted = _dbContext.NOTICE_STORE.ToList().Any(t => t.TypeID == noticeId);

            if (isExisted && noticeId == 3 || isExisted && noticeId == 4 || isExisted && noticeId == 5 || isExisted && noticeId == 6)
            {
                return Json(new { Message = "Mục này đã có và chỉ được đăng 1 bài viết!" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new {Message = ""}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InsertHomeImage()
        {
            if (Session["LibrarianName"] == null)
            {
                return RedirectToAction("LibrarianLogin", "Login");
            }

            var info = new DirectoryInfo(Server.MapPath("~/AllContent/home-images"));
            ViewBag.ImgList = info.GetFiles().OrderByDescending(t => t.CreationTime).ToList();
           
            return View();
        }

        [HttpPost]
        public ActionResult InsertHomeImage(HttpPostedFileBase imgHome)
        {
            if (Session["LibrarianName"] == null)
            {
                return RedirectToAction("LibrarianLogin", "Login");
            }

            try
            {
                if (imgHome == null)
                {
                    TempData["InsertImageFail"] = "Vui lòng chọn 1 ảnh";
                }
                else if (!CheckExtensionFile(imgHome.FileName))
                {
                    TempData["InsertImageFail"] = "Vui lòng chọn file có định dạng là ảnh. Ví dụ: .jpg, .png";
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/AllContent/home-images"), imgHome.FileName);
                    imgHome.SaveAs(path);
                    TempData["InsertImageSuccess"] = "Thêm ảnh trang chủ OPAC thành công!";
                }
            }
            catch (Exception)
            {
                TempData["InsertImageFail"] = "Có lỗi khi thêm ảnh";
            }

            return RedirectToAction("InsertHomeImage");
        }

        [HttpPost]
        public ActionResult DeleteImage(string fileNameList)
        {
            var info = new DirectoryInfo(Server.MapPath("~/AllContent/home-images"));
            var fileList = info.GetFiles().ToList();
            string[] getFileList = fileNameList.Trim().Split(' ');

            foreach (var img in fileList.ToList())
            {
                string temp = img.Name;
                temp = Regex.Replace(temp, @"\s+", "").ToLower();
                foreach (var img2 in getFileList)
                {
                    string temp2 = Regex.Replace(img2, @"\s+", "").ToLower(); ;
                    if (temp.Contains(temp2))
                    {
                        img.Delete();
                    }
                }
            }

            return Json(new { Message = "Xóa ảnh thành công!!" }, JsonRequestBehavior.AllowGet);
        }

        //Get day, month, year to initiate datetime
        private Tuple<int, int, int> ConvertToDate(string dateTime)
        {
            int year = 1;
            int month = 1;
            int day = 1;
            if (!string.IsNullOrEmpty(dateTime.Trim()))
            {
                var timeList = dateTime.Split('-');
                //Variable represent year
                year = Convert.ToInt32(timeList[0]);
                //Variable represent month
                month = Convert.ToInt32(timeList[1]);
                //Variable represent day
                day = Convert.ToInt32(timeList[2]);
            }

            return new Tuple<int, int, int>(year, month, day);
        }

        //The null DateTime is default time 0001-01-01
        private bool CheckNullDateTime(DateTime time)
        {
            return time == default(DateTime);
        }

        //Required image file
        private bool CheckExtensionFile(string path)
        {
            string[] extensionImageList =
            {
                ".tiff", ".svgz", ".pjp", ".webp.", ".jpeg", ".ico", ".jfif", ".gif", ".tif", ".xbm",
                ".pjpeg", ".bmp", ".jpg", ".svg", ".png", ".dib"
            };
            string extension = Path.GetExtension(path);

            return extensionImageList.Any(imgExtension => extension != null && extension.Equals(imgExtension));
        }
    }
}