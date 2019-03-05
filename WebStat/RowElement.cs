using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace WebStat
{
    public class RowElement : MapElement
    {
        public TreeNode Node { get; private set; }
        public RowElement(TreeNode node) : base(node.GetArea())
        {
            Node = node;
        }
    }
}