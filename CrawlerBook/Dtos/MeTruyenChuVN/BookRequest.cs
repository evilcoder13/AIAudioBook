﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerBook.Dtos.MeTruyenChuVN
{
    public class BookRequest
    {
        public string name {  get; set; }
        public string urlChapterOfBook { get; set; }
        public string urlReaderBook { get; set; }

        public BookRequest(string name, string urlReaderBook, string urlChapterOfBook)
        {
            this.name = name;
            this.urlReaderBook = urlReaderBook;
            this.urlChapterOfBook = urlChapterOfBook;
        }
    }
}
