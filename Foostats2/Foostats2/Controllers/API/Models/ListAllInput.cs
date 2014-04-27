using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Foostats2.Controllers.API
{
    public class ListAllInput
    {
        public string OrderBy { get; set; }
        public bool Desc { get; set; }
        public int Limit { get; set; }
        public int StartIndex { get; set; }
        public bool IncludeExtendedData { get; set; }
        public string SearchKey { get; set; }
        public string SearchTerm { get; set; }
    }
}