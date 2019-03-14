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
        public int nodesCount;
        private double koef;
        private double minSize = 50;

        public LayBuilder(TreeNode rootNode,int nodesCount,double width, double height)
        {
            this.nodesCount = nodesCount;
            mapRows = new List<MapRow>();
            this.rootNode = rootNode;
            mapWidth = width;
            mapHeight = height;
        }
        public IEnumerable<MapRow> getLayRows()
        {
            if (mapWidth * mapHeight == 0)
                return null;
            var children = (from ch in rootNode.Children select new RowElement(ch)).OrderByDescending(x=>x.Value).Take(nodesCount).ToList();
            currentArea = (from ch in children select ch.Value).Sum();
            double widthRatio = minSize / mapWidth;
            double heightRatio = minSize / mapWidth;
            children = (from ch in children where ch.Value / currentArea >= widthRatio*heightRatio select ch).ToList();
            currentArea = (from ch in children select ch.Value).Sum();
            koef = (mapWidth * mapHeight) / currentArea;
            currentArea = currentArea * koef;
            children.ForEach(x => x.Value *= koef);
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
                if(lastRow.Orientation==Orientation.Horizontal)
                {
                    left = lastRow.Left;
                    top = lastRow.Height + lastRow.Top;
                }
                else
                {
                    left = lastRow.Width + lastRow.Left;
                    top = lastRow.Top;
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
            temp.Add(first.Value);
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
                if (row.Orientation == Orientation.Horizontal)
                {
                    mapHeight -= row.Height;
                }
                else
                {
                    mapWidth -= row.Width;
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
