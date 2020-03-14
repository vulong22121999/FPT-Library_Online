using Libol.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Libol.SupportClass;
using Zoom.Net;
using Zoom.Net.YazSharp;
using Libol.EntityResult;

namespace Libol.Controllers
{
    public class Z3950Controller : Controller
    {
        LibolEntities le = new LibolEntities();
        // GET: Z3950
        public ActionResult SearchZ3950()
        {
            List<SelectListItem> lib = new List<SelectListItem>
            {
                new SelectListItem { Text = "Hãy chọn máy chủ", Value = "" }
            };
            foreach (var l in le.ILL_Z3950_SERVERS.ToList())
            {
                lib.Add(new SelectListItem { Text = l.Name, Value = l.ID.ToString() });
            }
            ViewData["lib"] = lib;

            return View();
        }

        public ActionResult DisplayZ3950(string strServerName, string strFielName1, string strFielName2, string strFielName3, string strFielVaslue1,
            string strFielVaslue2, string strFielVaslue3, string strOprerator2, string strOprerator3)
        {
            string checkTemmp = "";
            try
            {
                var catalogueList = new List<Catalogue>();

                var z3950Connection = le.FPT_SP_Z3950_GET_HOST_PORT_DBNAME().ToList()
                    .FirstOrDefault(t => t.ID == Convert.ToInt32(strServerName.Trim()));


                var connection = new Connection(z3950Connection.Host, Convert.ToInt32(z3950Connection.Port))
                {
                    DatabaseName = z3950Connection.DBName,
                    Username = z3950Connection.Account,
                    Password = z3950Connection.Password,
                    Syntax = RecordSyntax.XML
                };

                connection.Connect();

                //string query = "@not @attr 1=4 \"Basic college mathematics\" @attr 1=30 \"Houghton Mifflin\"";
                string query = "";
                string operators = "";
                if (!string.IsNullOrEmpty(strFielVaslue1))
                {
                    query += strFielName1 + " ";
                    query += "\"" + strFielVaslue1 + "\"";
                }

                if (!string.IsNullOrEmpty(strFielVaslue2))
                {
                    operators += strOprerator2 + " ";
                    query += strFielName2 + " ";
                    query += "\"" + strFielVaslue2 + "\"";
                }

                if (!string.IsNullOrEmpty(strFielVaslue3))
                {
                    operators += strOprerator3 + " ";
                    query += "\"" + strFielVaslue3 + "\"";
                }
                query = operators + query;
                while (query.Contains("  "))
                {
                    query = query.Replace("  ", " ");
                }
                query = query.Trim();
                var q = new PrefixQuery(query);
                var results = connection.Search(q);
                IResultSet result = (ResultSet)connection.Search(q);

                for (uint i = 0; i < result.Size; i++)
                {
                    string temp = Encoding.UTF8.GetString(results[i].Content);
                    checkTemmp = temp;
                    Catalogue cat = new Catalogue();
                    XmlDocument doc = new XmlDocument();
                    if (string.IsNullOrEmpty(temp))
                    {
                        continue;
                    }
                    doc.LoadXml(temp);

                    if (doc.DocumentElement != null)
                    {
                        var nsManager = new XmlNamespaceManager(doc.NameTable);
                        nsManager.AddNamespace("ns", doc.DocumentElement.NamespaceURI);

                        XmlElement node =
                            (XmlElement)doc.SelectSingleNode("/ns:" + doc.DocumentElement.Name, nsManager);

                        foreach (XmlNode dataField in node.ChildNodes)
                        {
                            if (dataField.Name.Equals("datafield") && dataField.Attributes != null)
                            {
                                if (dataField.Attributes["tag"].Value.Equals("020"))
                                {
                                    foreach (XmlNode subField in dataField.ChildNodes)
                                    {
                                        if (subField.Name.Equals("subfield") && subField.Attributes != null)
                                        {
                                            if (subField.Attributes["code"].Value.Equals("a"))
                                            {
                                                cat.ChiSoISBN = subField.InnerText;
                                            }
                                        }
                                    }
                                }

                                if (dataField.Attributes["tag"].Value.Equals("022"))
                                {
                                    foreach (XmlNode subField in dataField.ChildNodes)
                                    {
                                        if (subField.Name.Equals("subfield") && subField.Attributes != null)
                                        {
                                            if (subField.Attributes["code"].Value.Equals("a"))
                                            {
                                                cat.ChiSoISSN = subField.InnerText;
                                            }
                                        }
                                    }
                                }

                                if (dataField.Attributes["tag"].Value.Equals("040"))
                                {
                                    foreach (XmlNode subField in dataField.ChildNodes)
                                    {
                                        if (subField.Name.Equals("subfield") && subField.Attributes != null)
                                        {
                                            if (subField.Attributes["code"].Value.Equals("a"))
                                            {
                                                cat.CoQuanBienMucGoc = subField.InnerText;
                                            }
                                        }
                                    }
                                }

                                if (dataField.Attributes["tag"].Value.Equals("041"))
                                {
                                    foreach (XmlNode subField in dataField.ChildNodes)
                                    {
                                        if (subField.Name.Equals("subfield") && subField.Attributes != null)
                                        {
                                            if (subField.Attributes["code"].Value.Equals("a"))
                                            {
                                                cat.MaNgonNgu = subField.InnerText;
                                            }
                                        }
                                    }
                                }

                                if (dataField.Attributes["tag"].Value.Equals("044"))
                                {
                                    foreach (XmlNode subField in dataField.ChildNodes)
                                    {
                                        if (subField.Name.Equals("subfield") && subField.Attributes != null)
                                        {
                                            if (subField.Attributes["code"].Value.Equals("a"))
                                            {
                                                cat.MaNuocSanXuat = subField.InnerText;
                                            }
                                        }
                                    }
                                }

                                if (dataField.Attributes["tag"].Value.Equals("082"))
                                {
                                    foreach (XmlNode subField in dataField.ChildNodes)
                                    {
                                        if (subField.Name.Equals("subfield") && subField.Attributes != null)
                                        {
                                            if (subField.Attributes["code"].Value.Equals("a"))
                                            {
                                                cat.ChiSoPhanLoai = subField.InnerText;
                                            }
                                        }
                                    }
                                }

                                if (dataField.Attributes["tag"].Value.Equals("090"))
                                {
                                    foreach (XmlNode subField in dataField.ChildNodes)
                                    {
                                        if (subField.Name.Equals("subfield") && subField.Attributes != null)
                                        {
                                            if (subField.Attributes["code"].Value.Equals("a"))
                                            {
                                                cat.ChiSoPhanLoaiCucBo = subField.InnerText;
                                            }

                                            if (subField.Attributes["code"].Value.Equals("b"))
                                            {
                                                cat.ChiSoCutterCucBo = subField.InnerText;
                                            }
                                        }
                                    }
                                }

                                if (dataField.Attributes["tag"].Value.Equals("100"))
                                {
                                    foreach (XmlNode subField in dataField.ChildNodes)
                                    {
                                        if (subField.Name.Equals("subfield") && subField.Attributes != null)
                                        {
                                            if (subField.Attributes["code"].Value.Equals("a"))
                                            {
                                                cat.HoTenRieng = subField.InnerText;
                                            }
                                        }
                                    }
                                }

                                if (dataField.Attributes["tag"].Value.Equals("110"))
                                {
                                    foreach (XmlNode subField in dataField.ChildNodes)
                                    {
                                        if (subField.Name.Equals("subfield") && subField.Attributes != null)
                                        {
                                            if (subField.Attributes["code"].Value.Equals("a"))
                                            {
                                                cat.TenTapTheHoacPhapNhan = subField.InnerText;
                                            }
                                        }
                                    }
                                }

                                if (dataField.Attributes["tag"].Value.Equals("245"))
                                {
                                    foreach (XmlNode subField in dataField.ChildNodes)
                                    {
                                        if (subField.Name.Equals("subfield") && subField.Attributes != null)
                                        {
                                            if (subField.Attributes["code"].Value.Equals("a"))
                                            {
                                                cat.NhanDeChinh = subField.InnerText;
                                            }

                                            if (subField.Attributes["code"].Value.Equals("b"))
                                            {
                                                //cat.NhanDeSongSong = subField.InnerText;
                                                cat.PhuDe = subField.InnerText;
                                            }

                                            if (subField.Attributes["code"].Value.Equals("c"))
                                            {
                                                cat.ThongTinTrachNhiem = subField.InnerText;
                                            }

                                            if (subField.Attributes["code"].Value.Equals("n"))
                                            {
                                                cat.SoPhanMuc = subField.InnerText;
                                            }

                                            if (subField.Attributes["code"].Value.Equals("p"))
                                            {
                                                cat.TenPhanMuc = subField.InnerText;
                                            }
                                        }
                                    }
                                }

                                if (dataField.Attributes["tag"].Value.Equals("246"))
                                {
                                    foreach (XmlNode subField in dataField.ChildNodes)
                                    {
                                        if (subField.Name.Equals("subfield") && subField.Attributes != null)
                                        {
                                            if (subField.Attributes["code"].Value.Equals("a"))
                                            {
                                                cat.NhanDeHopLe = subField.InnerText;
                                            }
                                        }
                                    }
                                }

                                if (dataField.Attributes["tag"].Value.Equals("250"))
                                {
                                    foreach (XmlNode subField in dataField.ChildNodes)
                                    {
                                        if (subField.Name.Equals("subfield") && subField.Attributes != null)
                                        {
                                            if (subField.Attributes["code"].Value.Equals("a"))
                                            {
                                                cat.ThongTinLanXuatBan = subField.InnerText;
                                            }
                                        }
                                    }
                                }

                                if (dataField.Attributes["tag"].Value.Equals("260"))
                                {
                                    foreach (XmlNode subField in dataField.ChildNodes)
                                    {
                                        if (subField.Name.Equals("subfield") && subField.Attributes != null)
                                        {
                                            if (subField.Attributes["code"].Value.Equals("a"))
                                            {
                                                cat.NoiXuatBan = subField.InnerText;
                                            }

                                            if (subField.Attributes["code"].Value.Equals("b"))
                                            {
                                                cat.NhaXuatBan = subField.InnerText;
                                            }

                                            if (subField.Attributes["code"].Value.Equals("c"))
                                            {
                                                cat.NgayThangXuatBan = subField.InnerText;
                                            }
                                        }
                                    }
                                }

                                if (dataField.Attributes["tag"].Value.Equals("300"))
                                {
                                    foreach (XmlNode subField in dataField.ChildNodes)
                                    {
                                        if (subField.Name.Equals("subfield") && subField.Attributes != null)
                                        {
                                            if (subField.Attributes["code"].Value.Equals("a"))
                                            {
                                                cat.KhoiLuongVatLy = subField.InnerText;
                                            }

                                            if (subField.Attributes["code"].Value.Equals("b"))
                                            {
                                                cat.DacDiemVatLyKhac = subField.InnerText;
                                            }

                                            if (subField.Attributes["code"].Value.Equals("c"))
                                            {
                                                cat.Kho = subField.InnerText;
                                            }

                                            if (subField.Attributes["code"].Value.Equals("e"))
                                            {
                                                cat.TuLieuDiKem = subField.InnerText;
                                            }
                                        }
                                    }
                                }

                                if (dataField.Attributes["tag"].Value.Equals("490"))
                                {
                                    foreach (XmlNode subField in dataField.ChildNodes)
                                    {
                                        if (subField.Name.Equals("subfield") && subField.Attributes != null)
                                        {
                                            if (subField.Attributes["code"].Value.Equals("a"))
                                            {
                                                cat.ThongTinTungThu = subField.InnerText;
                                            }
                                        }
                                    }
                                }

                                if (dataField.Attributes["tag"].Value.Equals("500"))
                                {
                                    foreach (XmlNode subField in dataField.ChildNodes)
                                    {
                                        if (subField.Name.Equals("subfield") && subField.Attributes != null)
                                        {
                                            if (subField.Attributes["code"].Value.Equals("a"))
                                            {
                                                cat.GhiChuChung = subField.InnerText;
                                            }
                                        }
                                    }
                                }

                                if (dataField.Attributes["tag"].Value.Equals("520"))
                                {
                                    foreach (XmlNode subField in dataField.ChildNodes)
                                    {
                                        if (subField.Name.Equals("subfield") && subField.Attributes != null)
                                        {
                                            if (subField.Attributes["code"].Value.Equals("a"))
                                            {
                                                cat.GhiChuTomTat = subField.InnerText;
                                            }
                                        }
                                    }
                                }

                                if (dataField.Attributes["tag"].Value.Equals("650"))
                                {
                                    foreach (XmlNode subField in dataField.ChildNodes)
                                    {
                                        if (subField.Name.Equals("subfield") && subField.Attributes != null)
                                        {
                                            if (subField.Attributes["code"].Value.Equals("a"))
                                            {
                                                cat.ThuatNguChuDiem = subField.InnerText.Replace(",", "::");
                                            }
                                        }
                                    }
                                }

                                if (dataField.Attributes["tag"].Value.Equals("653"))
                                {
                                    foreach (XmlNode subField in dataField.ChildNodes)
                                    {
                                        if (subField.Name.Equals("subfield") && subField.Attributes != null)
                                        {
                                            if (subField.Attributes["code"].Value.Equals("a"))
                                            {
                                                cat.ThuatNguKhongKiemSoat = subField.InnerText.Replace(",", "::");
                                            }
                                        }
                                    }
                                }

                                if (dataField.Attributes["tag"].Value.Equals("700"))
                                {
                                    foreach (XmlNode subField in dataField.ChildNodes)
                                    {
                                        if (subField.Name.Equals("subfield") && subField.Attributes != null)
                                        {
                                            if (subField.Attributes["code"].Value.Equals("a"))
                                            {
                                                cat.TenRieng = subField.InnerText;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        catalogueList.Add(cat);
                    }
                }

                ViewBag.catalog = catalogueList;
            }
            catch (Zoom.Net.Bib1Exception ex)
            {
                TempData["ConnectError"] = "Không thể tìm kiếm nội dung này!";
                ErrorController error = new ErrorController();
                string path = "Z3950/DisplayZ3950";
                error.Log(path, ex.GetType().Name, ex.Message, ex.StackTrace);
                return View();
            }
            catch (Exception e)
            {
                TempData["ConnectError"] = "Không thể kết nối đến máy chủ này!";
                ErrorController error = new ErrorController();
                string path = "Z3950/DisplayZ3950";
                error.Log(path, e.GetType().Name, e.Message, e.StackTrace);
                return View();
            }

            return View();
        }
        public ActionResult SendCatalogue(string buttonID)
        {

            TempData["checkGet3950"] = buttonID;
            return RedirectToAction("AddNewCatalogue", "Catalogue");
        }

        public ActionResult Z3950Servers()
        {
            List<FPT_SP_GET_SERVERS_Z3950_Result> listZ3950Server = le.FPT_SP_GET_SERVERS_Z3950().ToList();
            ViewBag.listZ3950Server = listZ3950Server;
            return View();
        }

        public JsonResult LoadServers()
        {
            List<FPT_SP_GET_SERVERS_Z3950_Result> listZ3950Server = le.FPT_SP_GET_SERVERS_Z3950().ToList();

            return Json(listZ3950Server, JsonRequestBehavior.AllowGet); ;
        }

        public JsonResult DeleteServer(string strID)
        {
            string message = "";
            try
            {
                int id = Int16.Parse(strID);
                le.FPT_SP_Z3950_DELETE_SERVER(id);
            }
            catch (Exception e)
            {
                message = "Xóa thất bại!";
                ErrorController error = new ErrorController();
                string path = "Z3950/DeleteServer";
                error.Log(path, e.GetType().Name, e.Message, e.StackTrace);
                return Json(new { Message = message }, JsonRequestBehavior.AllowGet);
            }

            message = "Xóa thành công!";
            return Json(new { Message = message }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServerForEdit(string strID)
        {
            List<FPT_SP_GET_SERVERS_Z3950_Result> listZ3950Server = le.FPT_SP_GET_SERVERS_Z3950().ToList();
            FPT_SP_GET_SERVERS_Z3950_Result server = new FPT_SP_GET_SERVERS_Z3950_Result();
            int id = Int16.Parse(strID);
            foreach (var item in listZ3950Server)
            {
                if (item.ID == id)
                {
                    server = item;
                    break;
                }
            }
            return Json(server, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddServer(string strName, string strHost, string strPort, string strAccount, string strPassword, string strDBName, string strDescription)
        {
            string message = "";
            try
            {
                string name = strName.Trim();
                string host = strHost.Trim();
                int port = Int16.Parse(strPort);
                string account = strAccount.Trim();
                string password = strPassword.Trim();
                string dbName = strDBName.Trim();
                string description = strDescription.Trim();
                le.FPT_SP_Z3950_ADD_NEW_SERVER(name, host, port, account, password, dbName, description);
            }
            catch (Exception e)
            {
                message = "Thêm thất bại.";
                ErrorController error = new ErrorController();
                string path = "Z3950/AddServer";
                error.Log(path, e.GetType().Name, e.Message, e.StackTrace);
                return Json(new { Message = message }, JsonRequestBehavior.AllowGet);
               
            }

            message = "Thêm thành công!";
            return Json(new { Message = message }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditServer(string strID, string strName, string strHost, string strPort, string strAccount, string strPassword, string strDBName, string strDescription)
        {
            string message = "";
            try
            {
                int id = Int16.Parse(strID);
                string name = strName.Trim();
                string host = strHost.Trim();
                int port = Int16.Parse(strPort);
                string account = strAccount.Trim();
                string password = strPassword.Trim();
                string dbName = strDBName.Trim();
                string description = strDescription.Trim();
                le.FPT_SP_Z3950_EDIT_SERVER(id, name, host, port, account, password, dbName, description);
            }
            catch (Exception e)
            {
                message = "Sửa thất bại.";
                ErrorController error = new ErrorController();
                string path = "Z3950/EditServer";
                error.Log(path, e.GetType().Name, e.Message, e.StackTrace);
                return Json(new { Message = message }, JsonRequestBehavior.AllowGet);
                
            }

            message = "Sửa thành công!";
            return Json(new { Message = message }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TestConnection(string strName, string strHost, string strPort, string strAccount, string strPassword, string strDBName)
        {

            string message = "";
            try
            {
                string name = strName.Trim();
                string host = strHost.Trim();
                int port = Int32.Parse(strPort);
                string account = strAccount.Trim();
                string password = strPassword.Trim();
                string dbName = strDBName.Trim();
                var connection = new Connection(host, port)
                {
                    DatabaseName = dbName,
                    Username = account,
                    Password = password,
                    Syntax = RecordSyntax.XML
                };

                connection.Connect();
                message = "Kết nối thành công!";
            }
            catch (Exception e)
            {
                    message = "Kết nối thất bại";
                    ErrorController error = new ErrorController();
                    string path = "Z3950/TestConnection";
                    error.Log(path, e.GetType().Name, e.Message, e.StackTrace);
                    return Json(new { Message = message}, JsonRequestBehavior.AllowGet);
                

            }
            return Json(new { Message = message }, JsonRequestBehavior.AllowGet);
        }
    }
}