using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WebStat
{
    public class MapRow : MapElement
    {
        private List<RowElement> Elements;
        private double rectHeight;
        private double rectWidth;
        private double fullArea;
        public Orientation Orientation;
        public bool Completed { get; private set; }
        public MapRow(double left, double top, Orientation orientation, double emptyRectHeight, double emptyRectWidth,double fullArea) : base(0)
        {
            Elements = new List<RowElement>();
            this.Left = left;
            this.Top = top;
            this.Orientation = orientation;
            rectHeight = emptyRectHeight;
            rectWidth = emptyRectWidth;
            this.fullArea = fullArea;
            if (orientation == Orientation.Horizontal)
            {
                Width = emptyRectWidth;
            }
            else
            {
                Height = emptyRectHeight;
            }
            Completed = false;
        }
        private void getVariableSideLength()
        {
            if (Orientation == Orientation.Horizontal)
            {
                Height = rectHeight * (getArea() / fullArea);
            }
            else
            {
                Width = rectWidth * (getArea() / fullArea);
            }
        }
        public double getArea()
        {
            double sum = Elements.Sum(x => x.Value);
            return sum;
        }
        public List<double> getElementsValues()
        {
            var elementsValues = (from e in Elements select e.Value).ToList();
            return elementsValues;
        }
        public void AddElement(RowElement element)
        {
            if (Completed == false)
            {
                Elements.Add(element);
                Value += element.Value;
                getVariableSideLength();
            }
        }
        public void CompleteRow()
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    Elements[i].Height = Height;
                    Elements[i].Width = Width * (Elements[i].Value / getArea());
                    Elements[i].Top = Top;
                    if (i == 0)
                    {
                        Elements[i].Left = Left;
                    }
                    else
                    {
                        Elements[i].Left = Elements[i - 1].Left + Elements[i - 1].Width;
                    }
                }
                else
                {
                    Elements[i].Width = Width;
                    Elements[i].Height = Height * (Elements[i].Value / getArea());
                    Elements[i].Left = Left;
                    if (i == 0)
                    {
                        Elements[i].Top = Top;
                    }
                    else
                    {
                        Elements[i].Top = Elements[i - 1].Top + Elements[i - 1].Height;
                    }

                }
            }
            Completed = true;
        }
        public IEnumerable<RowElement> GetElements()
        {
            return Elements;
        }
    }
}