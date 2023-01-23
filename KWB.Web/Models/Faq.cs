using System.ComponentModel.DataAnnotations;

namespace KWB.Web.Models
{
    public class Faq
    {
        [Key]
        public int FaqID { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public bool? Enable { get; set; }
    }
}
