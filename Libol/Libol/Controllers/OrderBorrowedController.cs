using Libol.Models;
using Libol.SupportClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace Libol.Controllers
{
    public class OrderBorrowedController : Controller
    {
        LibolEntities le = new LibolEntities();
        FormatHoldingTitle format = new FormatHoldingTitle();
        private readonly String WAITING = "Đợi sách";
        private readonly String ORDER_SUCCESS = "Thành công";
        private readonly String CANCEL = "Hết hạn";
        // GET: OrderBorrowed
        public ActionResult OrderBorrowed()
        {

            return View();
        }
        [HttpPost]
        public PartialViewResult GetOrderBorrowed()
        {
            List<FPT_CIR_HOLDING_GET_ALL_Result> list = le.FPT_CIR_HOLDING_GET_ALL().ToList();
            foreach (FPT_CIR_HOLDING_GET_ALL_Result temp in list)
            {
                temp.BOOKNAME = format.getContent(temp.BOOKNAME.Split('$'), "a");
                if (temp.InTurn == false && temp.CheckMail == false)
                {
                    temp.ORDER_STATUS = WAITING;
                }
                else if ((temp.InTurn == true && temp.CheckMail == true) || (temp.InTurn == true && temp.CheckMail == false))
                {
                    temp.ORDER_STATUS = ORDER_SUCCESS;
                }
                else if ((temp.InTurn == false && temp.CheckMail == true))
                {
                    temp.ORDER_STATUS = CANCEL;
                }

            }
            //list.Sort();
            list = list.OrderByDescending(t => t.ORDER_STATUS.Equals("Đợi sách")).ThenByDescending(t => t.ORDER_STATUS.Equals("Thành công")).ToList();
            ViewBag.listPatron = list;
            return PartialView("GetOrderBorrowed");
        }
        [HttpPost]
        public PartialViewResult GetOrderBorrowedSearched(bool strCheckOrder, bool strOrderCancel, string strDateFrom,
                                                            string strDateTo, bool strTitleCheck, bool strPatronCheck, string strSearchContent)
        {
            List<FPT_CIR_HOLDING_GET_ALL_Result> list = le.FPT_CIR_HOLDING_SEARCH(strCheckOrder, strOrderCancel, strDateFrom, strDateTo, strTitleCheck, strPatronCheck, strSearchContent).ToList();
            foreach (FPT_CIR_HOLDING_GET_ALL_Result temp in list)
            {
                temp.BOOKNAME = format.getContent(temp.BOOKNAME.Split('$'), "a");
                if (temp.InTurn == false && temp.CheckMail == false)
                {
                    temp.ORDER_STATUS = WAITING;
                }
                else if ((temp.InTurn == true && temp.CheckMail == true) || (temp.InTurn == true && temp.CheckMail == false))
                {
                    temp.ORDER_STATUS = ORDER_SUCCESS;
                }
                else if ((temp.InTurn == false && temp.CheckMail == true))
                {
                    temp.ORDER_STATUS = CANCEL;
                }

            }
            list.Sort();
            ViewBag.listPatron = list;
            return PartialView("GetOrderBorrowed");
        }
        [HttpPost]
        public JsonResult deleteOrder(string strOrderID)
        {
            List<FPT_CIR_HOLDING_GET_ALL_Result> list = le.FPT_CIR_HOLDING_GET_ALL().ToList();
            strOrderID = strOrderID.Trim();
            strOrderID = strOrderID.Substring(0, strOrderID.LastIndexOf(','));
            string[] orderArr = strOrderID.Split(',');
            foreach (string item in orderArr)
            {
                le.ExcuteSQL("update CIR_HOLDING set CheckMail=1, InTurn=0 where id =" + item);
            }

            return Json("", JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult sendMail(string strOrderID)
        {
            List<FPT_CIR_HOLDING_GET_ALL_Result> list = le.FPT_CIR_HOLDING_GET_ALL().ToList();
            strOrderID = strOrderID.Trim();
            strOrderID = strOrderID.Substring(0, strOrderID.LastIndexOf(','));
            string[] orderArr = strOrderID.Split(',');
            foreach (string item in orderArr)
            {
                CIR_HOLDING order = le.CIR_HOLDING.Where(a => a.ID.ToString() == item).First();
                if (order.TimeOutDate == null)
                {
                    order.TimeOutDate = DateTime.Now.AddDays(7);
                }
                string date = Convert.ToDateTime(order.TimeOutDate).ToString("dd/MM/yyyy");
                CIR_PATRON patron = le.CIR_PATRON.Where(a => a.Code == order.PatronCode).First();
                string name = patron.FirstName + " " + patron.MiddleName + " " + patron.LastName;
                DateTime dateTime = DateTime.Now.AddDays(7);
                le.ExcuteSQL("update CIR_HOLDING set CheckMail=1, InTurn=1, TimeOutDate='" + dateTime.ToString() + "' where id =" + item);
                string bookname = "";
                foreach(FPT_CIR_HOLDING_GET_ALL_Result temp in list)
                {
                    if (temp.ID.ToString().Equals(item))
                    {
                        bookname = temp.BOOKNAME;
                        bookname= format.getContent(bookname.Split('$'), "a");
                        break;
                    }
                }
                sendmail(patron.Email, name, date,bookname);
            }

            return Json("", JsonRequestBehavior.AllowGet);

        }

        private void sendmail(string email, string name, string timeout, string bookname)
        {
            var fromAddress = new MailAddress("santintt197@gmail.com", "FPTU Library");
            var toAddress = new MailAddress(email, "To Name");
            string subject = "";
            string content = "";
            const string fromPassword = "trongthang197";
            XmlDocument doc = new XmlDocument();
            string filePath = Server.MapPath("~/MailContent/OrderBookMailForm.xml");
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
            content = content.Replace("name", name);
            content = content.Replace("timeoutdate", timeout);
            content = content.Replace("book", bookname);
            
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
                try { smtp.Send(message); }
                catch
                {

                }

            }
        }
    }
}