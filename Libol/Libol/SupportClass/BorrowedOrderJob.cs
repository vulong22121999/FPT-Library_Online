using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Quartz;
using Libol.Models;

namespace Libol.SupportClass
{
    public class BorrowedOrderJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            LibolEntities le = new LibolEntities();
            string today = DateTime.Now.ToString("yyyy/MM/dd");
            var cirHoldingList = le.CIR_HOLDING.ToList();
            if(cirHoldingList != null)
            {
                foreach (var c in cirHoldingList)
                {
                    string timeOutDate = Convert.ToDateTime(c.TimeOutDate).ToString("yyyy/MM/dd");
                    if ((c.InTurn == true && c.CheckMail == false) || (c.InTurn == true && c.CheckMail == true) && timeOutDate == today)
                    {
                        c.InTurn = false;
                        c.CheckMail = true;
                    }
                }
                le.SaveChanges();
            }
            
            await Console.Out.WriteLineAsync("BorrowedOrderJob is executing.");
        }
    }
}