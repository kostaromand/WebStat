using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Linq;

namespace WebStat
{
    public partial class MainWindow : Window
    {
        ApplicationViewModel viewModel;
        public MainWindow(TreeNode root,TreeInfo info)
        {
            InitializeComponent();
            viewModel = new ApplicationViewModel(this, root, info);
        }
        private void ShowTree_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ShowTree();
        }
    }
}
