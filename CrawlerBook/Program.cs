// See https://aka.ms/new-console-template for more information
using CrawlerBook;
using CrawlerBook.Dtos.MeTruyenChuVN;
using System.Text.Json;


MeTruyenChuVN meTruyenChuVN = new MeTruyenChuVN();
meTruyenChuVN.Crawling();


Console.ReadKey();