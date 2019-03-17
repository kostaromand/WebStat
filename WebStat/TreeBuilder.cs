using System.Collections.Generic;
using System.Linq;
namespace WebStat
{
    public class TreeBuilder
    {
        private int id;
        private List<RequestObject> requestObjects;
        private TreeNode root;   //корень дерева
        public int topRequestsCount { get; set; } //количество реквестов в топе
        public DataReader Reader { get; set; }
        public TreeInfo Info { get; set; }
        public NodeCalculator nodeCalculator;
        public TreeBuilder(int topRequestsCount,DataReader dataReader,TreeInfo info)
        {
            this.Info = info;
            this.topRequestsCount = topRequestsCount;
            this.Reader = dataReader;
            nodeCalculator = new NodeCalculator();
        }
        public TreeNode GetTree()
        {
            id = 0;
            root = new TreeNode(null, id, "WebStat");
            requestObjects = Reader.ReadData().ToList();
            BuildTree();
            nodeCalculator.CalcTopRequests(root, topRequestsCount);
            nodeCalculator.CalcArea(root);
            nodeCalculator.CalcTopLinks(root, topRequestsCount);
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
                    current.AddLinkToNode(request.link, request.Position);
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