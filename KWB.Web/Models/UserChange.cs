using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KWB.Web.Models
{
    public class UserChange
    {
        [Key]
        public int UserChangeID { get; set; }
        public int UserID { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public string? InterfaceID { get; set; }
        [NotMapped]
        public string? UserName { get; set; }
        [NotMapped]
        public string? InterfaceName { get; set; }
    }
}
