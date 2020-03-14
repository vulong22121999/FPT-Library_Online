using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OPAC.Models;
using System.Net;
using System.Net.Mail;
using System.Xml;
using OPAC.Dao;
using XCrypt;

namespace OPAC.Controllers
{
    public class LoginController : Controller
    {
        private OpacEntities db = new OpacEntities();
        private PatronDao dao = new PatronDao();

        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var checkUser = db.SP_OPAC_CHECK_PATRON_CARD(username, password).FirstOrDefault();
            
            if (checkUser != null)
            {
                int userId = checkUser.ID;
                Session["ID"] = userId;
                Session["Info"] = checkUser;
                Session["OnHolding"] = checkUser.Code;

                return RedirectToAction("PatronAfterLoginPage", "InformationPatron");
            }

            ViewData["Notification"] = "Tên đăng nhập/mật khẩu không đúng!";
            return View();
        }

        [HttpPost]
        public ActionResult LoginForDetailSearch(string username, string password, string itemId)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return Json(new { Message = "Vui lòng nhập tên đăng nhập hoặc mật khẩu!" }, JsonRequestBehavior.AllowGet);
            }

            var checkUser = db.SP_OPAC_CHECK_PATRON_CARD(username, password).FirstOrDefault();

            if (checkUser != null)
            {
                int userId = checkUser.ID;
                Session["ID"] = userId;
                Session["Info"] = checkUser;
                Session["OnHolding"] = checkUser.Code;

                return Json(new { Message = "Success" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = "Tên đăng nhập/mật khẩu không đúng!" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            Session.Remove("ID");
            Session.Remove("Info");
            Session.Remove("OnHolding");
            TempData["LoginRequest"] = null;

            return RedirectToAction("Home", "Home");
        }

        public ActionResult ForgetPassword(string email, string studentCode)
        {
            try
            {
                if (string.IsNullOrEmpty(email.Trim()) || string.IsNullOrEmpty(studentCode.Trim()))
                {
                    TempData["EmptyError"] = "Email hoặc mã sinh viên không được để trống!";
                    return RedirectToAction("Login");
                }
                string newPassword = dao.RandomPassword();
                if (dao.UpdatePasswordByEmail(newPassword, email, studentCode) == 1)
                {
                    var fromAddress = new MailAddress("santintt197@gmail.com", "FPT Library");
                    var toAddress = new MailAddress(email, "To Name");
                    string subject = "";
                    string content = "";
                    const string fromPassword = "trongthang197";
                    string filePath = Server.MapPath("~/MailContent/ChangePasswordAccount.xml");
                    XmlDocument doc = new XmlDocument();
                    doc.Load(filePath);
                    foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                    {
                        if (node.Name.Equals("subject"))
                        {
                            subject = node.InnerText;
                        }
                        if (node.Name.Equals("content"))
                        {
                            content = node.InnerText;
                        }
                    }

                    //Thay thế từ trong file xml thành data load từ database
                    content = content.Replace("pass", newPassword);
                  
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                    };
                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = content,
                    })
                    {
                        message.IsBodyHtml = true;
                        smtp.Send(message);
                    }

                    TempData["ChangePassword"] = "Mật khẩu mới đã được reset!";
                    return RedirectToAction("Login");
                }
                
            }
            catch (FormatException)
            {
                TempData["MailError"] = "Địa chỉ email hoặc mã sinh viên không đúng!";
                return RedirectToAction("Login");
            }

            TempData["ChangePasswordError"] = "Mã sinh viên hoặc email không đúng!";
            return RedirectToAction("Login");
        }

        public ActionResult LibrarianLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LibrarianLogin(string username, string password)
        {
            string passEncrypt = new XCryptEngine(XCryptEngine.AlgorithmType.MD5).Encrypt(password, "pl");
            var librarianAccount = db.SP_SYS_USER_LOGIN(username, passEncrypt).FirstOrDefault();

            if (librarianAccount == null)
            {
                ViewData["LibraryNotification"] = "Tên đăng nhập/mật khẩu không đúng!";
                return View();
            }

            var librarianInfo = librarianAccount.Name;
            Session["LibrarianName"] = librarianInfo;
            
            return RedirectToAction("CreateNotification", "Notification");
        }

        public ActionResult LibrarianLogout()
        {
            Session.Remove("LibrarianName");

            return RedirectToAction("LibrarianLogin");
        }
    }
}