using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPAC.Models.SupportClass
{
    public class AdvancedSearch
    {
        public string LibraryId { get; set; }
        public string LocationId { get; set; }
        public string TxtSearch1 { get; set; }
        public string TxtSearch2 { get; set; }
        public string TxtSearch3 { get; set; }
        public string TxtSearch4 { get; set; }
        public string SearchType1 { get; set; }
        public string SearchType2 { get; set; }
        public string SearchType3 { get; set; }
        public string SearchType4 { get; set; }
        public string Condition1 { get; set; }
        public string Condition2 { get; set; }
        public string Condition3 { get; set; }
        public string DocumentType { get; set; }
        public string OrderBy { get; set; }
    }
}