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
        bool f = false;
        string filePath = "";
        int topRequest = 5;
        int maxColorNum = 200;
        int minColorNum = 50;
        TreeNode root;
        TreeBuilder builder;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void getPath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePath = ofd.FileName;
                builder = new TreeBuilder(topRequest, new CSVDataReader(filePath),new GroupBuilder());
                root = builder.GetTree();
                WebStatCanvas.Children.Clear();
                CreateTreeMap(root, WebStatCanvas,1,true);
            }
        }
        void CreateTreeMap(TreeNode node, Canvas panel, int level, bool isFirst = false)
        {
            double w = panel.ActualWidth == 0 ? panel.Width:panel.ActualWidth;
            double h = panel.ActualHeight == 0 ? panel.Height : panel.ActualHeight;
            LayBuilder layBuilder = new LayBuilder(node, w, h);
            var rows = layBuilder.getLayRows();
            if (rows == null)
                return;
            Random rand = new Random();
            foreach (var row in rows)
            {
                foreach (var elem in row.GetElements())
                {
                    var rowElement = (RowElement)elem;
                    Color color;
                    if (isFirst == true)
                    {
                        byte r = (byte)(rand.Next(minColorNum, maxColorNum));
                        byte g = (byte)(rand.Next(minColorNum, maxColorNum));
                        byte b = (byte)(rand.Next(minColorNum, maxColorNum));
                        color = Color.FromRgb(r, g, b);
                    }
                    else
                    {
                        color = ((SolidColorBrush)panel.Background).Color;
                    }
                    DockPanel dockPanel = new DockPanel();
                    TextBlock title = new TextBlock();
                    Color auxColor = color;
                    auxColor.R -= (byte)(10 * level);
                    auxColor.G -= (byte)(10 * level);
                    auxColor.B -= (byte)(10 * level);
                    title.Background = new SolidColorBrush(auxColor);
                    title.Text = rowElement.Node.ShortName;
                    title.TextAlignment = TextAlignment.Center;
                    title.FontSize = 16;
                    title.Height = 20;
                    title.Foreground = new SolidColorBrush(Colors.White);
                    title.SetValue(DockPanel.DockProperty, Dock.Top);
                    dockPanel.Children.Add(title);
                    dockPanel.Width = rowElement.width;
                    dockPanel.LastChildFill = true;
                    dockPanel.Height = rowElement.height;
                    dockPanel.SetValue(Canvas.LeftProperty, rowElement.left);
                    dockPanel.SetValue(Canvas.TopProperty, rowElement.top);
                    dockPanel.LastChildFill = true;
                    Border border = new Border();
                    border.BorderThickness = new Thickness(1);
                    border.BorderBrush = new SolidColorBrush(auxColor);
                    Canvas canvas = new Canvas();
                    canvas.Background = new SolidColorBrush(color);
                    border.Child = canvas;
                    dockPanel.Children.Add(border); 
                    Popup popup = new Popup();
                    popup.Placement = PlacementMode.Mouse;
                    DockPanel popupPanel = new DockPanel();
                    popupPanel.Width = 250;
                    popupPanel.Height = 250;
                    TextBlock popupTitle = new TextBlock();
                    popupTitle.Text = rowElement.Node.ShortName;
                    popupTitle.TextWrapping = TextWrapping.Wrap;
                    popupTitle.TextAlignment = TextAlignment.Center;
                    popupTitle.FontSize = 16;
                    popupTitle.Foreground = new SolidColorBrush(Colors.White);
                    popupTitle.SetValue(DockPanel.DockProperty, Dock.Top);
                    TextBlock popupNotation = new TextBlock();
                    popupNotation.Text = "Топ Запросов:";
                    popupNotation.TextAlignment = TextAlignment.Left;
                    popupNotation.FontSize = 16;
                    popupNotation.Foreground = new SolidColorBrush(Colors.White);
                    popupNotation.SetValue(DockPanel.DockProperty, Dock.Top);
                    ListBox listBoxNames = new ListBox();
                    listBoxNames.Background = new SolidColorBrush(auxColor);
                    listBoxNames.Foreground = new SolidColorBrush(Colors.White);
                    listBoxNames.FontSize = 16;
                    listBoxNames.HorizontalAlignment = HorizontalAlignment.Stretch;
                    if (node.TopRequests.Count!=0)
                        listBoxNames.ItemsSource = (from r in node.TopRequests select r.Item1 + " " + r.Item2).ToArray();
                    listBoxNames.SetValue(DockPanel.DockProperty, Dock.Left);
                    popupPanel.Children.Add(popupTitle);
                    popupPanel.Children.Add(popupNotation);
                    popupPanel.Children.Add(listBoxNames);
                    popupPanel.Background = new SolidColorBrush(auxColor);
                    Border borderPopup = new Border();
                    borderPopup.BorderThickness = new Thickness(1);
                    borderPopup.BorderBrush = new SolidColorBrush(Colors.Coral);
                    borderPopup.Child = popupPanel;
                    popup.Child = borderPopup;
                    title.MouseEnter += (s,e)=> { popup.IsOpen = true; };
                    title.MouseLeave += (s, e) => { popup.IsOpen = false; };
                    panel.Children.Add(dockPanel);
                    double canvasHeight = dockPanel.Height - title.Height;
                    if (canvasHeight <= 0 || dockPanel.Width <= 0)
                        continue;
                    canvas.Height = canvasHeight;
                    canvas.Width = dockPanel.Width;
                    title.Loaded += (s, e) => {
                            CreateTreeMap(rowElement.Node, canvas, level + 1, false); } ;
                }
            }
        }
    }
}
