using System.Collections.Generic;
using System.Linq;
namespace WebStat
{
    public class TreeBuilder
    {
        private int id;
        private List<RequestObject> requestObjects = new List<RequestObject>();
        private TreeNode root;   //корень дерева
        public int topRequestsCount { get; set; } //количество реквестов в топе
        public DataReader Reader { get; set; }
        public TreeInfo Info { get; set; }
        public TreeBuilder(int topRequestsCount,DataReader dataReader,TreeInfo info)
        {
            Info = info;
            this.topRequestsCount = topRequestsCount;
            Reader = dataReader;
        }
        public TreeNode GetTree()
        {
            id = 0;
            root = new TreeNode(null, id, "WebStat");
            requestObjects = Reader.ReadData().ToList();
            BuildTree();
            root.CalculateTopRequests(topRequestsCount);
            root.GetArea();
            return root;
        }

        void BuildTree()
        {
            id = root.Id;
            TreeNode current;
            requestObjects.ForEach(
                request =>
                {
                    current = AddNodesToTree(root, Info.getNodeNamesSequence(request));
                    current.AddRequestToNode(request.Request, request.PhraseFrequency, request.AccurateFrequency);
                });
        }

        TreeNode AddNodesToTree(TreeNode node, string[] groups) //добавление узлов к дереву
        {
            //имена узлов
            string[] nodes = groups;
            TreeNode current = node;
            TreeNode child;
            foreach (var element in nodes)
            {
                child = current.Children.Find(x => x.ShortName == element);
                if (child == null)
                {
                    child = new TreeNode(current, id++, element);
                }
                current = child;
            }
            //возвращение добавленного листа(последнего узла)
            return current;
        }
    }
}