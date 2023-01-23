using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models.Response
{
    public class ResponseThingToDo
    {
        public int ID { get; set; }
        public string Value { get; set; }
        public int? PlaceID { get; set; }
        public List<ThingsToDo> ThingsToDoDB { get; set; }
        public List<string> LocationTagDB { get; set; }
        public string LocationTagSelected { get; set; }
    }
}
