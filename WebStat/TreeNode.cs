using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace WebStat
{
    public class TreeNode
    {
        public List<Tuple<string, int>> TopRequests { get; private set; }
        private long area; //площадь
        public int Id { get; private set; } 
        public string Name { get; private set; } //имя узла
        public int ParentId { get; private set; } //ID родительского узла
        public List<TreeNode> Children { get; set; } //список дочерних узлов
        public string ShortName { get; private set; } //короткое имя узла
        //Cписок топа запросов Key - название запроса, value - частота
        [JsonIgnore]
        public TreeNode Parent { get; private set; } //родительский узел
        //Cписок запросов Key - название запроса, value - частота
        [JsonIgnore]
        public Dictionary<string, int> Requests { get; set; } 

        public long GetArea()
        {
            if (area == 0)
            {
                CalculateArea();
            }
            return area;
        }
    
        private void CalculateArea() //посчитать площадь узла
        {
            if (Children.Count == 0)
            {
                area = (from d in Requests select d.Value).Sum();
            }
            else
            {
                area = (from ch in Children select ch.GetArea()).Sum();
            }
        }
        public TreeNode(TreeNode parent, int id, string shortName) 
        {
            TopRequests = new List<Tuple<string, int>>();
            Parent = parent;
            Id = id;
            if (parent != null)
            {
                ParentId = parent.Id;
            }
            ShortName = shortName;
            Requests = new Dictionary<string, int>();
            Children = new List<TreeNode>();
            if (parent == null)
            {
                Name = shortName;
            }
            else
            {
                parent.Children.Add(this);
                Name = parent.ShortName + "." + shortName;
            }
        }
        //добавление запроса в узел
        public void AddRequestToNode(string request,int phraseFreq, int accurateFreq) 
        {
            int freq = accurateFreq == 0 ? phraseFreq : accurateFreq;
            if (!Requests.ContainsKey(request))
            {
                Requests.Add(request, freq);
            }
            else if(Requests[request]<freq)
            {
                Requests[request] = freq;
            }
        }

        public void CalculateTopRequests(int count) //найти топ реквестов для узла
        {
            _calculateTopRequests(count).ToList();
        }
        //Топ N запросов узла
        IEnumerable<Tuple<string,int>> _calculateTopRequests(int count) //метод для просчета топа для узла
        {
            IEnumerable<Tuple<string, int>> current;
            if (Children.Count==0)
            {
                current = (from item in Requests.OrderByDescending(x => x.Value)
                               select new Tuple<string, int>(item.Key, item.Value)).Take(count);
                TopRequests = current.ToList();
                return current;
            }
            else
            {
                IEnumerable<Tuple<string,int>> mergeList = new List<Tuple<string,int>>();
                foreach(var child in Children)
                {
                    mergeList = mergeList.Concat(child._calculateTopRequests(count));
                }
                current = mergeList.OrderByDescending(x => x.Item2).Take(count);
                TopRequests = current.ToList();
                return current;
            }
        }
        public override bool Equals(object obj)
        {
            var node = obj as TreeNode;
            return node != null &&
                   Name == node.Name;

        }   
    }
}
