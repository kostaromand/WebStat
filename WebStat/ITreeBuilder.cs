using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebStat
{
    public interface ITreeBuilder
    {
        void BuildTree(TreeNode root, List<RequestObject> requestObjects);
    }
}
