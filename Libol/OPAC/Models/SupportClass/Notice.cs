using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPAC.Models.SupportClass
{
    public class Notice
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public System.DateTime CreateTime { get; set; }
        public int TypeID { get; set; }
        public int LibID { get; set; }
    }
}