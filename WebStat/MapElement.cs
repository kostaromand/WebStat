﻿namespace WebStat
{
    public class MapElement
    {
        public double Value { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public MapElement(double value)
        {
            this.Value = value;
        }
    }
}