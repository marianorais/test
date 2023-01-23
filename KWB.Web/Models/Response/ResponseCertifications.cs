using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models.Response
{
    public class ResponseCertifications
    {
        [Key]
        public int CertificationID { get; set; }
        public string Name { get; set; }
        public string? Website { get; set; }
        public IFormFile? Icono { get; set; }
        public string? WindowLocation { get; set; }
        public List<string>? CertNames { get; set; }
        public List<Certification>? CertificationsDB { get; set; }
    }
}
