using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models
{
    public class Certification
    {
        [Key]
        public int CertificationID { get; set; }
        public string Name { get; set; }
        public string? Website { get; set; }
        public string? Icono { get; set; }
    }
}
