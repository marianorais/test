using System;
using System.Collections.Generic;

namespace KWB.Web.Models.Table
{
    public class TablePlace
    {
        public string? Draw { get; set; }
        public int? RecordsTotal { get; set; }
        public int? RecordsFiltered { get; set; }
        public List<Tag> Data { get; set; }
    }
}
