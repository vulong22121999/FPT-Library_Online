using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPAC.Models.SupportClass
{
    public class CounterStatistics
    {
        public int Today { get; set; }
        public int Yesterday { get; set; }
        public int ThisWeek { get; set; }
        public int LastWeek { get; set; }
        public int ThisMonth { get; set; }
        public int LastMonth { get; set; }

        public int GetTotal()
        {
            return ThisMonth + LastMonth;
        }
    }
}