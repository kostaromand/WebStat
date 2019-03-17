using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
namespace WebStat
{
    public class TreeNode
    {
        public Dictionary<string, int> TopRequests { get; set; }
        public Dictionary<string, int> TopLinks { get;set; }
        public long Area { get; set; } //площадь
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
        public Dictionary<string, int> Requests { get; private set; }
        public Dictionary<string, int> Links { get; private set; }
        public TreeNode(TreeNode parent, int id, string shortName)
        {
            TopRequests = new Dictionary<string, int>();
            Parent = parent;
            Id = id;
            if (parent != null)
            {
                ParentId = parent.Id;
            }
            ShortName = shortName;
            Requests = new Dictionary<string, int>();
            Links = new Dictionary<string, int>();
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
        //добавление ссылки в узел
        public void AddLinkToNode(string link, int position)
        {
            if (!Links.ContainsKey(link))
            {
                Links.Add(link, position);
            }
            else if (Links[link] < position)
            {
                Links[link] = position;
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
