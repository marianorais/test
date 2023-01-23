using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models
{
    public class Caracteristicas
    {
        [Key]
        public int CaractID { get; set; }
        public string Caracteristica { get; set; }
        public string? Valor { get; set; }
        public int? PlaceID { get; set; }
        public int? CityID { get; set; }
    }
}
