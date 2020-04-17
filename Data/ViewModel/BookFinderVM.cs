using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.ViewModel
{
    public class BookFinderVM
    {
        public IEnumerable<BookFinderVM> data { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public DateTime Published { get; set; }
        public string URL { get; set; }
        public long ISBN { get; set; }
        public int length { get; set; }
        public int filterLength { get; set; }
    }
}
