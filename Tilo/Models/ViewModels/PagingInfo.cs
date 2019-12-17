using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilo.Models.ViewModels
{
    public class PagingInfo
    {
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);

        public PagingInfo(int totalItems, int current, int itemsPerPage)
        {           
            TotalItems = totalItems;
            CurrentPage = current;
            ItemsPerPage = itemsPerPage;
        }
        public PagingInfo()
        {
        }
    }

}
