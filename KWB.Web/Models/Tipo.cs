using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models
{
    public class Tipo
    {
        [Key]
        public int TipoID { get; set; }
        public string? Name { get; set; }
    }
}
