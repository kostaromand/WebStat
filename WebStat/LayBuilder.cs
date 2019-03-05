using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WebStat
{
    public class LayBuilder
    {
        private List<MapRow> mapRows;
        private TreeNode rootNode;
        private double mapWidth;
        private double mapHeight;
        private double currentArea;
        private double koef;
        public LayBuilder(TreeNode rootNode,double width, double height)
        {
            mapRows = new List<MapRow>();
            this.rootNode = rootNode;
            mapWidth = width;
            mapHeight = height;
            currentArea = (from ch in rootNode.Children select ch.GetArea()).Sum();
            koef = (width * height)/currentArea;
            currentArea = currentArea * koef;
        }
        public IEnumerable<MapRow> getLayRows()
        {
            if (mapWidth * mapHeight == 0)
                return null;
            var children = (from ch in rootNode.Children select new RowElement(ch)).OrderByDescending(x=>x.value).ToList();
            children.ForEach(x => x.value *= koef);
            var row = getRow();
            squarify(children, row, Math.Min(mapHeight, mapWidth));
            return mapRows;
        }
        private MapRow getRow()
        {
            double left, top;
            if(mapRows.Count==0)
            {
                left = 0;
                top = 0;
            }
            else
            {
                var lastRow = mapRows.Last();
                if(lastRow.orientation==Orientation.Horizontal)
                {
                    left = lastRow.left;
                    top = lastRow.height + lastRow.top;
                }
                else
                {
                    left = lastRow.width + lastRow.left;
                    top = lastRow.top;
                }
            }
            Orientation orientation;
            if (mapWidth > mapHeight)
            {
                orientation = Orientation.Vertical;
            }
            else
                orientation = Orientation.Horizontal;
            var mapRow = new MapRow(left, top, orientation, mapHeight, mapWidth, currentArea);
            return mapRow;
        }
        private void squarify(List<RowElement> children, MapRow row,double width)
        {
            if (children.Count == 0)
            {
                row.CompleteRow();
                mapRows.Add(row);
                return;
            }        
            var first = children[0];
            var values = row.getElementsValues();
            var temp = values.Select(x => x).ToList();
            temp.Add(first.value);
            if (getMaxRatio(row.getElementsValues(), width) >= getMaxRatio(temp, width))
            {
                row.AddElement(first);
                children.RemoveAt(0);
                squarify(children,row,width);
            }
            else
            {
                row.CompleteRow();
                currentArea = currentArea - row.getArea();
                if (row.orientation == Orientation.Horizontal)
                {
                    mapHeight -= row.height;
                }
                else
                {
                    mapWidth -= row.width;
                }
                mapRows.Add(row);
                var newRow = getRow();
                squarify(children, newRow, Math.Min(mapHeight, mapWidth));
            }
            
        }
        private double getMaxRatio(List<double> areas, double width)
        {
            double squareWidth = width * width;
            double squareSum = Math.Pow(areas.Sum(), 2.0);
            double result = double.PositiveInfinity;
            if (areas.Count != 0)
            {
                var a = (squareWidth * areas.Max());
                var b = squareSum;
                var c = (squareWidth * areas.Min());
                result = Math.Max((squareWidth * areas.Max()) / squareSum, squareSum / (squareWidth * areas.Min()));
            }
            return result;
        }
    }
}
