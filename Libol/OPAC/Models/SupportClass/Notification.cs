using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPAC.Models.SupportClass
{
    public class Notification
    {
        public int ID { get; set; }
        public int Count { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CreateTime { get; set; }
        public string TypeName { get; set; }
        public string LibraryName { get; set; }
    }
}