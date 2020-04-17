using Data.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Model
{
    public class BookFinder :BaseModel
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public DateTime Published { get; set; }
        public string URL { get; set; }
        public long ISBN { get; set; }
    }
}
