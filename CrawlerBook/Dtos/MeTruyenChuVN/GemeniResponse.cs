using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerBook.Dtos.MeTruyenChuVN
{
    public class Part
    {
        public string Text { get; set; }
    }

    public class Content
    {
        public List<Part> Parts { get; set; }
        public string Role { get; set; }
    }

    public class Candidate
    {
        public Content Content { get; set; }
        public string FinishReason { get; set; }
        public double AvgLogprobs { get; set; }
    }


    public class GemeniResponse
    {
        public List<Candidate> Candidates { get; set; }
    }
}
