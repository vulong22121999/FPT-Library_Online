using OPAC.Dao;
using OPAC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using OPAC.Models.SupportClass;
using System.Globalization;

namespace OPAC.Controllers
{
    public class SearchBookController : Controller
    {
        private SearchDao dao = new SearchDao();

        // GET: DetailBook
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("Detail")]
        public ActionResult DetailBook(int itemID)
        {
            ViewBag.OnHoldingBook = dao.GetOnHoldingBook(itemID);
            ViewBag.TotalBook = dao.GetTotalBook(itemID);
            ViewBag.FreeBook = dao.GetFreeBook(itemID);
            ViewBag.InforCopyNumber = dao.GetInforCopyNumberList(itemID);
            ViewBag.RelatedTerm = dao.FPT_SP_OPAC_GET_RELATED_TERMS_LIST(itemID);
            ViewBag.BookTitle = dao.GetItemTitle(itemID);
            ViewBag.FullBookInfo = dao.GetFullInforBook(itemID);
            ViewBag.Summary = dao.GetSummary(itemID);
            ViewBag.RecordType = dao.GetRecordType(itemID);
            ViewBag.ItemID = itemID;
            if (ViewBag.CurrentIndex != null)
            {
                ViewBag.CurrentIndex = (int)TempData.Peek("Index");
            }

            try
            {
                var terms = dao.FPT_SP_OPAC_GET_RELATED_TERMS_LIST(itemID).Where(s => s.TermType.Equals("DDC"))
                    .FirstOrDefault();

                var ddc = terms.DisplayEntry;
                if (ddc.Contains("$a"))
                {
                    ddc = ddc.Replace("$a", "");
                }

                if (ddc.Contains("$b"))
                {
                    ddc = ddc.Replace("$b", " ");
                }

                ViewBag.DDC = ddc;
                ViewBag.OriginalDDC = terms.DisplayEntry;
            }
            catch (NullReferenceException)
            {
                ViewBag.DDC = "";
            }

            TempData["itemID"] = itemID;

            //TempData["code"] = code;

            return View(dao.SP_CATA_GET_CONTENTS_OF_ITEMS_LIST(itemID, 0));
        }

        //GET: Detail advanced search
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("DetailAdvancedSearch")]
        public ActionResult DetailAdvancedSearchBook(int itemID)
        {
            ViewBag.OnHoldingBook = dao.GetOnHoldingBook(itemID);
            ViewBag.TotalBook = dao.GetTotalBook(itemID);
            ViewBag.FreeBook = dao.GetFreeBook(itemID);
            ViewBag.InforCopyNumber = dao.GetInforCopyNumberList(itemID);
            ViewBag.RelatedTerm = dao.FPT_SP_OPAC_GET_RELATED_TERMS_LIST(itemID);
            ViewBag.BookTitle = dao.GetItemTitle(itemID);
            ViewBag.FullBookInfo = dao.GetFullInforBook(itemID);
            ViewBag.Summary = dao.GetSummary(itemID);
            ViewBag.RecordType = dao.GetRecordType(itemID);
            ViewBag.ItemID = itemID;
            if (ViewBag.CurrentIndex != null)
            {
                ViewBag.CurrentIndex = (int)TempData.Peek("Index");
            }

            try
            {
                var terms = dao.FPT_SP_OPAC_GET_RELATED_TERMS_LIST(itemID).Where(s => s.TermType.Equals("DDC"))
                    .FirstOrDefault();

                var ddc = terms.DisplayEntry;
                if (ddc.Contains("$a"))
                {
                    ddc = ddc.Replace("$a", "");
                }

                if (ddc.Contains("$b"))
                {
                    ddc = ddc.Replace("$b", " ");
                }

                ViewBag.DDC = ddc;
                ViewBag.OriginalDDC = terms.DisplayEntry;
            }
            catch (NullReferenceException)
            {
                ViewBag.DDC = "";
            }

            TempData["itemID"] = itemID;

            return View(dao.SP_CATA_GET_CONTENTS_OF_ITEMS_LIST(itemID, 0));
        }

        public ActionResult SearchItemByIndex(int index)
        {
            int itemIdOfIndex;
            if (TempData.Peek("Flag") == null)
            {
                var list = (List<FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK_Result>) TempData.Peek("FullAdvancedSearchBook");
                itemIdOfIndex = list[index - 1].ID;

                return RedirectToAction("DetailAdvancedSearchBook", new {itemID = itemIdOfIndex});
            }

            bool flag = (bool) TempData.Peek("Flag");
            if (flag)
            {
                var list = (List<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_Result>) TempData.Peek("FullBookList");
                itemIdOfIndex = list[index - 1].ID;
            }
            else
            {
                var list =
                    (List<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD_Result>) TempData.Peek("FullBookListByKeyword");
                itemIdOfIndex = list[index - 1].ID;
            }

            return RedirectToAction("DetailBook", new {itemID = itemIdOfIndex});
        }

        [HttpPost]
        public ActionResult GetKeySearch(OptionModel model, string selectOption)
        {
            try
            {
                if (model.SearchingText.Trim().Equals(""))
                {
                    ViewBag.EmptyKeword = "";
                    TempData["errorMessage"] = "Ô tìm kiếm không được để trống";
                    return RedirectToAction("Home", "Home");
                }
                else
                {
                    TempData["key"] = model.SearchingText.Trim();
                    TempData["option"] = selectOption;
                }
            }
            catch (NullReferenceException)
            {
                ViewBag.EmptyKeword = "";
                TempData["errorMessage"] = "Ô tìm kiếm không được để trống";
                return RedirectToAction("Home", "Home");
            }

            return RedirectToAction("SearchBook", new {page = 1});
        }

        public ActionResult SearchBy(string keyWord, int searchBy)
        {
            TempData["keyWord"] = keyWord.Trim();
            TempData["searchBy"] = searchBy;
            return RedirectToAction("SearchBookByKeyWord", new {page = 1});
        }

        //The page returns result of normal search list
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("SearchResult")]
        public ActionResult SearchBook(int page)
        {
            string key = TempData.Peek("key").ToString();
            string option = TempData.Peek("option").ToString();
            int maxItemInOnePage = 30;
            int numberResult = dao.GetNumberResult(key, option);
            ViewBag.Key = key;
            ViewBag.ItemInOnePage = maxItemInOnePage;
            TempData["PageNo"] = page;
            TempData["CountResultList"] = numberResult;
            TempData["Flag"] = true;
            TempData["FullBookList"] = dao.GetFullSearchingBook(key, option);

            return View(dao.GetSearchingBook(key, option, page, maxItemInOnePage));
        }

        //The page returns result of keyword search list
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("SearchResultByKeyword")]
        public ActionResult SearchBookByKeyWord(int page)
        {
            string keyWord = TempData.Peek("keyWord").ToString();
            int searchBy = (int)TempData.Peek("searchBy");
            int maxItemInOnePage = 30;
            int numberResultByKeyWord = dao.GetNumberResultByKeyWord(keyWord, page, maxItemInOnePage, searchBy);
            ViewBag.KeyWord = keyWord;
            ViewBag.ItemInOnePage = maxItemInOnePage;
            TempData["PageNo"] = page;
            TempData["CountResultList"] = numberResultByKeyWord;
            TempData["Flag"] = false;
            TempData["FullBookListByKeyword"] =
                dao.GetFullSearchingBookByKeyWord(keyWord, searchBy);

            return View(dao.GetSearchingBookByKeyWord(keyWord, page, maxItemInOnePage, searchBy));
        }

        //The view page of advanced search list
        [Route("AdvancedSearch")]
        public ActionResult AdvancedSearchBook()
        {
            ViewBag.DocumentList = dao.GetDocumentType();
            var libraryList = new List<SP_GET_LIBRARY_Result>();
            var searchTypeList = dao.GetSearchTypeDicList();
            var shortList = new List<SelectListItem>();
            shortList.Insert(0, new SelectListItem() { Text = "Mọi trường", Value = "AllFields" });
            shortList.Insert(1, new SelectListItem() { Text = "Nhan đề", Value = "Title" });

            foreach (var item in dao.GetLibrary())
            {
                if (!string.IsNullOrWhiteSpace(item.Name))
                {
                    libraryList.Add(item);
                }
            }

            foreach (var item in searchTypeList)
            {
                shortList.Add(new SelectListItem() { Text = item.Name, Value = item.ID.ToString() });
            }

            ViewBag.LibraryList = libraryList;
            ViewBag.SearchType = shortList;

            return View();
        }

        //Get location of each library (Lấy danh sách kho từ thư viện)
        public JsonResult GetLocationByLibId(int libraryId)
        {
            var list = dao.GetLocation(libraryId);
            var listLocation = new List<SelectListItem>();
            list.Insert(0, new Location()
            {   
                ID = 0,
                Status = false,
                SymbolAndCodeLoc = "--------------Chọn kho--------------"
            });
            foreach (var item in list)
            {
                listLocation.Add(new SelectListItem() { Text = item.SymbolAndCodeLoc, Value = item.ID.ToString() });
            }
            return Json(new SelectList(listLocation, "Value", "Text"));
        }

        //Get all information of input by user
        [HttpPost]
        public ActionResult GetOptionAdvancedSearch(LibraryLocationModel model, string txtSearch1, string txtSearch2, 
            string txtSearch3, string txtSearch4, string libraryId, string searchType, string searchType2, string searchType3,
            string searchType4, string condition1, string condition2, string condition3, string documentType, string orderBy)
        {
            if (string.IsNullOrEmpty(txtSearch1.Trim()) && string.IsNullOrEmpty(txtSearch2.Trim()) &&
                string.IsNullOrEmpty(txtSearch3.Trim()) && string.IsNullOrEmpty(txtSearch4.Trim()))
            {
                TempData["errorMessage"] = "Từ khóa không được để trống!";
                return RedirectToAction("AdvancedSearchBook");
            }

            AdvancedSearch input = new AdvancedSearch()
            {
                LibraryId = libraryId,
                LocationId = model.LocationId,
                SearchType1 = searchType,
                SearchType2 = searchType2,
                SearchType3 = searchType3,
                SearchType4 = searchType4,
                TxtSearch1 = txtSearch1,
                TxtSearch2 = txtSearch2,
                TxtSearch3 = txtSearch3,
                TxtSearch4 = txtSearch4,
                Condition1 = condition1,
                Condition2 = condition2,
                Condition3 = condition3,
                DocumentType = documentType,
                OrderBy = orderBy
            };

            TempData["inputSearch"] = input;

            return RedirectToAction("AdvancedSearchResult", new {page = 1});
        }

        //The page returns the list of advanced search
        [Route("AdvancedSearchResult")]
        public ActionResult AdvancedSearchResult(int page)
        {
            var input = (AdvancedSearch) TempData.Peek("inputSearch");
            int maxItemInOnePage = 30;
            int numberResult = dao.GetNumberResultByAdvancedSearch(input);
            ViewBag.AdvancedSearchItemInOnePage = maxItemInOnePage;
            TempData["AdvancedCountResultList"] = numberResult;
            TempData["AdvancedSearchPageNo"] = page;
            TempData["Flag"] = null;
            TempData["FullAdvancedSearchBook"] = dao.GetFullAdvancedSearchBook(input);

            return View(dao.GetAdvancedSearchBook(input, page, maxItemInOnePage));
        }

        //Register to borrow book
        public ActionResult RegisterToBorrowBook(int userID, int itemID, int type)
        {
            if (type == 1)
            {
                string patronCode = "";
                int waitingBookId = 1;
                using (var dbContext = new OpacEntities())
                {
                    patronCode = (from t in dbContext.CIR_PATRON where t.ID == userID select t.Code).FirstOrDefault();
                    var lastIndex = dbContext.CIR_HOLDING.ToList().LastOrDefault();

                    if (lastIndex != null)
                    {
                        waitingBookId = lastIndex.ID + 1;
                    }
                }

                DateTime currentTime = DateTime.Now;
                DateTime getBookTime = currentTime.AddDays(7);

                CIR_HOLDING waitingBook = new CIR_HOLDING()
                {
                    ID = waitingBookId,
                    ItemID = itemID,
                    CheckMail = false,
                    InTurn = true,
                    CreatedDate = currentTime,
                    TimeOutDate = getBookTime,
                    PatronCode = patronCode
                };

                dao.RegisterToBorrow(waitingBook);
            }
            else
            {
                string patronCode = "";
                int waitingBookId = 1;
                using (var dbContext = new OpacEntities())
                {
                    patronCode = (from t in dbContext.CIR_PATRON where t.ID == userID select t.Code).FirstOrDefault();
                    var lastIndex = dbContext.CIR_HOLDING.ToList().LastOrDefault();

                    if (lastIndex != null)
                    {
                        waitingBookId = lastIndex.ID + 1;
                    }
                }

                DateTime currentTime = DateTime.Now;

                CIR_HOLDING waitingBook = new CIR_HOLDING()
                {
                    ID = waitingBookId,
                    ItemID = itemID,
                    CheckMail = false,
                    InTurn = false,
                    CreatedDate = currentTime,
                    PatronCode = patronCode
                };

                dao.RegisterToBorrow(waitingBook);
            }
            
            TempData["BorrowSuccess"] = "Đặt mượn sách thành công!";

            return RedirectToAction("RegisterToBorrowBookPage", "InformationPatron");
        }
    }
}