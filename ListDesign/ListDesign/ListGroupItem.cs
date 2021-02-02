using System;
using System.Collections.Generic;

namespace ListDesign
{
    public class ListGroupItem : List<string>
    {
        public DateTime ItemDate { get; set; }

        public List<string> Items => this;
    }
}
