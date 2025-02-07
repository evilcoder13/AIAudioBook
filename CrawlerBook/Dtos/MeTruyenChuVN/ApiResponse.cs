using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerBook.Dtos.MeTruyenChuVN
{
    public class ApiResponse
    {
        public bool? success { get; set; }
        public string message { get; set; }
        public string data { get; set; }
    }
}
