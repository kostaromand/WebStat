using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebStat
{
    public class GroupBuilder : ITreeBuilder
    {
        int id;
        public void BuildTree(TreeNode root, List<RequestObject> requestObjects)
        {
            id = root.Id;
            TreeNode current;
            requestObjects.ForEach(
                request =>
                {
                    current = AddNodesToTree(root, request.Group);
                    current.AddRequestToNode(request.Request, request.PhraseFrequency, request.AccurateFrequency);
                });
        }
        TreeNode AddNodesToTree(TreeNode node, string str) //добавление узлов к дереву
        {
            //имена узлов
            string[] nodes = str.Split('.');
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
