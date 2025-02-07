using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerBook.Dtos.MeTruyenChuVN
{
    public class Book
    {
        public string name {  get; set; }
        public string urlReaderBook { get; set; }
        public List<Chapter> chapters { get; set; }
    }
}
