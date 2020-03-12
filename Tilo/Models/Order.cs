using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tilo.Models
{
    public class Order
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Please enter your name")]
        public string CustomerName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage="Please enter your email")]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^\d{10}$",
            ErrorMessage = "Please enter your phone xxxxxxxxxx")]
        public string Phone { get; set; }
        public DateTime dateTime { get; set; }

        public IEnumerable<OrderLine> Lines { get; set; }
    }
}
