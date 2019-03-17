using System;
using System.Collections.Generic;
using System.Linq;
namespace WebStat
{
    public class NodeCalculator
    {
        public virtual void CalcTopRequests(TreeNode root,int count)
        {
            calculateTopRequests(root, count);
        }

        IEnumerable<Tuple<string, int>> calculateTopRequests(TreeNode root,int count) //метод для просчета топа для узла
        {
            IEnumerable<Tuple<string, int>> current;
            if (root.Children.Count == 0)
            {
                current = (from item in root.Requests.OrderByDescending(x => x.Value)
                           select new Tuple<string, int>(item.Key, item.Value)).Take(count);
                root.TopRequests = current.ToDictionary(x => x.Item1, x => x.Item2);
                return current;
            }
            else
            {
                IEnumerable<Tuple<string, int>> mergeList = new List<Tuple<string, int>>();
                foreach (var child in root.Children)
                {
                    mergeList = mergeList.Concat(calculateTopRequests(child,count));
                }
                var grouped = mergeList.OrderByDescending(x => x.Item2).GroupBy(x => x.Item1);
                var distinct = (from g in grouped select g.First()).Take(count);
                root.TopRequests = distinct.ToDictionary(x => x.Item1, x => x.Item2);
                return distinct;
            }
        }
        public virtual long CalcArea(TreeNode root)
        {
            if (root.Children.Count == 0)
            {
                root.Area = (from r in root.Requests select r.Value).Sum();
            }
            else
            {
                root.Area = (from ch in root.Children select CalcArea(ch)).Sum();
            }
            return root.Area;
        }
        public virtual void CalcTopLinks(TreeNode root, int count)
        {
            calcTopLinks(root, count);
        }
        IEnumerable<Tuple<string, int>> calcTopLinks(TreeNode root, int count)
        {
            IEnumerable<Tuple<string, int>> current;
            if (root.Children.Count == 0)
            {
                current = (from item in root.Links.OrderBy(x => x.Value)
                           select new Tuple<string, int>(item.Key, item.Value)).Take(count);
                root.TopLinks = current.ToDictionary(x => x.Item1, x => x.Item2);
                return current;
            }
            else
            {
                IEnumerable<Tuple<string, int>> mergeList = new List<Tuple<string, int>>();
                foreach (var child in root.Children)
                {
                    mergeList = mergeList.Concat(calcTopLinks(child, count));
                }
                var grouped = mergeList.OrderBy(x => x.Item2).GroupBy(x => x.Item1);
                var distinct = (from g in grouped select g.First()).Take(count);
                root.TopLinks = distinct.ToDictionary(x => x.Item1, x => x.Item2);
                return distinct;
            }
        }
    }
}