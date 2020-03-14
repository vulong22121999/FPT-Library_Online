using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using OPAC.Models;
using OPAC.Models.SupportClass;

namespace OPAC.Dao
{
    public class PatronDao
    {
        /// <summary>
        /// Update password
        /// </summary>
        /// <param name="password"></param>
        /// <param name="userID"></param>
        public void UpdatePassword(string password, int userID)
        {
            using (var dbContext = new OpacEntities())
            {
                var account = (from a in dbContext.CIR_PATRON where a.ID == userID select a).FirstOrDefault();

                if (account != null)
                {
                    account.Password = password;
                    dbContext.Entry(account).State = EntityState.Modified;
                    dbContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Update password via Email
        /// </summary>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <param name="studentCode"></param>
        /// <returns></returns>
        public int UpdatePasswordByEmail(string password, string email, string studentCode)
        {
            using (var dbContext = new OpacEntities())
            {
                var account = (from a in dbContext.CIR_PATRON
                    where a.Email.Equals(email) && a.Code.Equals(studentCode)
                    select a).FirstOrDefault();
                if (account != null)
                {
                    account.Password = password;
                    dbContext.Entry(account).State = EntityState.Modified;
                    dbContext.SaveChanges();
                    return 1;
                }
            }

            return -1;
        }

        /// <summary>
        /// Random new password with the length of 7
        /// </summary>
        /// <returns></returns>
        public string RandomPassword()
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            int passwordLength = 7;
            StringBuilder newPassword = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (passwordLength-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    newPassword.Append(valid[(int) (num % (uint) valid.Length)]);
                }
            }

            return newPassword.ToString().Trim();
        }

        /// <summary>
        /// Update extend date and count time of extend date
        /// </summary>
        /// <param name="newDate"></param>
        /// <param name="countRenew"></param>
        /// <param name="ID"></param>
        public void ExtendDate(string newDate, int countRenew, int ID)
        {
            using (var dbContext = new OpacEntities())
            {
                var item = (from t in dbContext.CIR_LOAN where t.ID == ID select t).FirstOrDefault();
                if (item != null)
                {
                    var tempDateTime = DateTime.ParseExact(newDate, "dd/MM/yyyy",
                        System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");
                    var newDateTime = DateTime.ParseExact(tempDateTime, "yyyy/MM/dd",
                        System.Globalization.CultureInfo.InvariantCulture);
                    item.DueDate = newDateTime;
                    item.RenewCount = (short) countRenew;
                    dbContext.Entry(item).State = EntityState.Modified;
                    dbContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Get number day for extend date
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="copyNumber"></param>
        /// <returns></returns>
        public static int GetRenewalPeriod(int itemID, string copyNumber)
        {
            using (var dbContext = new OpacEntities())
            {
                //Renew day for Textbooks are 1 week, Reference books with "Eng" language is 2 weeks, "Vie" is 1 week
                var renewalPeriod = 7;
                var bookType = dbContext.HOLDINGs.Where(t => t.ItemID == itemID).Select(t => t.CopyNumber).FirstOrDefault();

                if (bookType != null)
                {
                    bookType = bookType.Substring(0, 2);

                    if (bookType.ToLower().Contains("tk"))
                    {
                        var book = dbContext.FPT_SP_GET_LANGUAGE_CODE_ITEM(itemID).FirstOrDefault();

                        if (book != null && !book.ToLower().Trim().Contains("vie"))
                        {
                            renewalPeriod = 14;
                        }
                    }

                }

                return renewalPeriod;
            }
        }

        /// <summary>
        /// Get number of extension date
        /// </summary>
        /// <param name="cirID"></param>
        /// <returns></returns>
        public static int GetRenewCount(int cirID)
        {
            using (var dbContext = new OpacEntities())
            {
                var renewalPeriod = (from t in dbContext.CIR_LOAN
                    where t.ID == cirID
                    select t.RenewCount).FirstOrDefault();

                return renewalPeriod;
            }
        }

        /// <summary>
        /// Get history of borrowed books list
        /// </summary>
        /// <param name="patronID"></param>
        /// <returns></returns>
        public IEnumerable<FPT_SP_OPAC_GET_LOAN_HISTORY_Result> GetLoanHistoryList(int patronID)
        {
            using (var dbContext = new OpacEntities())
            {
                return dbContext.FPT_SP_OPAC_GET_LOAN_HISTORY(patronID).ToList();
            }
        }

        /// <summary>
        /// Get on holding book list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OnHoldingBook> GetOnHoldingList(string patronCode)
        {
            var onHoldingList = new List<OnHoldingBook>();
            using (var dbContext = new OpacEntities())
            {
                var list = dbContext.CIR_HOLDING.Where(t => t.PatronCode.Equals(patronCode)).ToList();

                foreach (var item in list)
                {
                    string title = dbContext.FIELD200S.Where(t => t.ItemID == item.ItemID && t.FieldCode.Equals("245"))
                        .Select(a => a.Content).FirstOrDefault();
                    title = title.Replace("$a", "").Replace("$b", " ").Replace("$c", " ")
                        .Replace("$e", " ").Replace("$p", " ").Replace("$n", " ");

                    int statusId = SetStatusId(item);
                    string status = NotifyStatus(statusId);

                    OnHoldingBook holding = new OnHoldingBook()
                    {
                        ItemID = Convert.ToInt32(item.ItemID),
                        Title = title,
                        CreatedDate = item.CreatedDate,
                        TimeOutDate = Convert.ToDateTime(item.TimeOutDate),
                        StatusId = statusId,
                        Status = status
                    };

                    onHoldingList.Add(holding);
                }
            }

            return onHoldingList;
        }

        /// <summary>
        /// Check duplicated request to borrow book
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="patronCode"></param>
        /// <returns></returns>
        public static bool IsExistedItemId(int itemId, string patronCode)
        {
            using (var dbContext = new OpacEntities())
            {
                return dbContext.CIR_HOLDING.Any(t => t.ItemID == itemId && t.PatronCode.Equals(patronCode)
                                                      && (t.InTurn == true && t.CheckMail ||
                                                          t.InTurn == true && !t.CheckMail ||
                                                          t.InTurn == false && !t.CheckMail));
            }
        }

        /// <summary>
        /// Check ID of loan type. TextBooks have ID: 13, 21, 22; else is References book
        /// </summary>
        /// <param name="loanTypeID"></param>
        /// <param name="renewCount"></param>
        /// <returns></returns>
        public static bool IsTextBook(int loanTypeID, int renewCount)
        {
            const int maxRenewForTextbook = 1;
            const int maxRenewForReferenceBook = 4;
            if (loanTypeID == 13 || loanTypeID == 21 || loanTypeID == 22)
            {
                //TextBooks will be renewed 1 time
                if (renewCount < maxRenewForTextbook)
                {
                    return true;
                }
            }
            else
            {
                //Reference books will be renewed 4 times
                if (renewCount < maxRenewForReferenceBook)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Patron only renews the book before 3 days at the expired date
        /// </summary>
        /// <param name="expiredTime"></param>
        /// <returns></returns>
        public static bool CheckRenewsBook(string expiredTime)
        {
            //The expiredTime parameter must have the format datetime as dd/MM/yyyy
            var splitTime = expiredTime.Split('/');
            int day = Convert.ToInt32(splitTime[0]);
            int month = Convert.ToInt32(splitTime[1]);
            int year = Convert.ToInt32(splitTime[2]);
            DateTime expiredDateTime = new DateTime(year, month, day);

            return DateTime.Now.Date >= expiredDateTime.Date.AddDays(-3) && DateTime.Now.Date <= expiredDateTime.Date;
        }

        /// <summary>
        /// Get content of lock reason patron's account
        /// </summary>
        /// <param name="patronCode"></param>
        /// <returns></returns>
        public static string GetLockReason(string patronCode)
        {
            using (var dbContext = new OpacEntities())
            {
                return dbContext.CIR_PATRON_LOCK.Where(t => t.PatronCode.ToLower().Equals(patronCode.ToLower()))
                    .Select(t => t.Note).FirstOrDefault();
            }
        }

        public static DateTime ConvertStringToDateTime(string datetime)
        {
            var splitTime = datetime.Split('/');
            int day = Convert.ToInt32(splitTime[0]);
            int month = Convert.ToInt32(splitTime[1]);
            int year = Convert.ToInt32(splitTime[2]);
            return new DateTime(year, month, day);
        }

        /*
         * Status ID:
         * 1. Inturn False, CheckMail False: patron register to borrow, but library is out of books, when book is available,
         * librarian will notify patron via Mail
         * 2. Inturn False, CheckMail True: when book is available, librarian will notify patron via Mail but patron has not
         * got the book
         * 3. Inturn True, CheckMail True: book is available to borrow
         * 4. Inturn True, CheckMail False: patron register to borrow successfully and can get the book
         */
        private int SetStatusId(CIR_HOLDING holding)
        {
            int statusId = 0;
            if (!Convert.ToBoolean(holding.InTurn) && !holding.CheckMail)
            {
                statusId = 1;
            }
            else if (!Convert.ToBoolean(holding.InTurn) && holding.CheckMail)
            {
                statusId = 2;
            }
            else if (Convert.ToBoolean(holding.InTurn) && holding.CheckMail)
            {
                statusId = 3;
            }
            else
            {
                statusId = 4;
            }

            return statusId;
        }

        //Status of registering to borrow book
        private string NotifyStatus(int statusId)
        {
            string status = "";
            if (statusId == 1)
            {
                status = "Đang chờ mượn";
            }
            else if (statusId == 2)
            {
                status = "Hết hạn";
            }
            else if (statusId == 3 || statusId == 4)
            {
                status = "Sẵn sàng mượn";
            }

            return status;
        }
    }
}