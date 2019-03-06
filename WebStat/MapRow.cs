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
        public Orientation orientation;
        public bool completed { get; private set; }
        public MapRow(double left, double top, Orientation orientation, double emptyRectHeight, double emptyRectWidth,double fullArea) : base(0)
        {
            Elements = new List<RowElement>();
            this.left = left;
            this.top = top;
            this.orientation = orientation;
            rectHeight = emptyRectHeight;
            rectWidth = emptyRectWidth;
            this.fullArea = fullArea;
            if (orientation == Orientation.Horizontal)
            {
                width = emptyRectWidth;
            }
            else
            {
                height = emptyRectHeight;
            }
            completed = false;
        }
        private void getVariableSideLength()
        {
            if (orientation == Orientation.Horizontal)
            {
                height = rectHeight * (getArea() / fullArea);
            }
            else
            {
                width = rectWidth * (getArea() / fullArea);
            }
        }
        public double getArea()
        {
            double sum = Elements.Sum(x => x.value);
            return sum;
        }
        public List<double> getElementsValues()
        {
            var elementsValues = (from e in Elements select e.value).ToList();
            return elementsValues;
        }
        public void AddElement(RowElement element)
        {
            if (completed == false)
            {
                Elements.Add(element);
                value += element.value;
                getVariableSideLength();
            }
        }
        public void CompleteRow()
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                if (orientation == Orientation.Horizontal)
                {
                    Elements[i].height = height;
                    Elements[i].width = width * (Elements[i].value / getArea());
                    Elements[i].top = top;
                    if (i == 0)
                    {
                        Elements[i].left = left;
                    }
                    else
                    {
                        Elements[i].left = Elements[i - 1].left + Elements[i - 1].width;
                    }
                }
                else
                {
                    Elements[i].width = width;
                    Elements[i].height = height * (Elements[i].value / getArea());
                    Elements[i].left = left;
                    if (i == 0)
                    {
                        Elements[i].top = top;
                    }
                    else
                    {
                        Elements[i].top = Elements[i - 1].top + Elements[i - 1].height;
                    }

                }
            }
            completed = true;
        }
        public IEnumerable<RowElement> GetElements()
        {
            return Elements;
        }
    }
}