using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilo.Models.ViewModels
{
    public class MailViewModel
    {
        public LinkedResource HeaderImage { get; set; }
        public string Content { get; set; }
        public LinkedResource FooterImage { get; set; }
    }
}
