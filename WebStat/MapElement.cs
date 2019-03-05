using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WebStat
{
    public class MapElement
    {
        public double value { get; set; }
        public double left { get; set; }
        public double top { get; set; }
        public double height { get; set; }
        public double width { get; set; }
        public MapElement(double value)
        {
            this.value = value;
        }
    }
}