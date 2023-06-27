using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.Models
{
    public class ErrorLog
    {
        [Key]
        public int Id { get; set; }

        public string ErrorMessage { get; set; }
        public DateTime DateTime { get; set; }

        public ErrorLog() { }
    }
}
