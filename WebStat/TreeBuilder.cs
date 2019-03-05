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
        public ITreeBuilder builder { get; set; }
        public TreeBuilder(int topRequests,DataReader dataReader,ITreeBuilder builder)
        {
            this.builder = builder;
            topRequestsCount = topRequests;
            Reader = dataReader;
        }
        public TreeNode GetTree()
        {
            id = 0;
            root = new TreeNode(null, id, "WebStat");
            requestObjects = Reader.ReadData().ToList();
            builder.BuildTree(root, requestObjects);
            root.CalculateTopRequests(topRequestsCount);
            root.CalculateArea();
            return root;
        }
    }
}