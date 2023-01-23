using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KWB.Web.Models
{
    public class ApiCallsRegister
    {
        [Key]
        public int ApiCallsRegisterID { get; set; }
        public string Name { get; set; }
        public int? Count { get; set; }
        public DateTime? Date { get; set; }
        [NotMapped]
        public int? Quantity { get; set; }
    }
}
