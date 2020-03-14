using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPAC.Models.SupportClass
{
    public class OnHoldingBook
    {
        public int ItemID { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime TimeOutDate { get; set; }
        public int StatusId { get; set; }
        public string Status { get; set; }
    }
}