using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WebStat
{
    public class ApplicationViewModel
    {
        public string t = "asd";
        MainWindow window;
        string filePath = "";
        int topRequest = 5;
        int nodesOnLevel = 5;
        int topRequestCount = 5;
        int maxColorNum = 200;
        int minColorNum = 50;
        TreeBuilder builder;
        public ApplicationViewModel(MainWindow window)
        {
            this.window = window;
        }
        public void getPathClick()
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePath = ofd.FileName;
                builder = new TreeBuilder(topRequest, new CSVDataReader(filePath), new GroupBuilder());
            }
        }

        public void ShowTree()
        {
            int customNodesOnLevel=0;
            bool checkOnNum = int.TryParse(window.NodesOnLelelTextBox.Text, out customNodesOnLevel);
            if (checkOnNum  && customNodesOnLevel>0 && customNodesOnLevel<10)
            {
                nodesOnLevel = customNodesOnLevel;
            }
            int cunstomTopRequest = 0;
            checkOnNum = int.TryParse(window.TopRequestCountTextBox.Text, out cunstomTopRequest);
            if (checkOnNum && cunstomTopRequest > 0 && cunstomTopRequest < 10)
            {
                topRequestCount = cunstomTopRequest;
            }
            if (builder!=null)
            {
                TreeNode root = builder.GetTree();
                window.WebStatCanvas.Children.Clear();
                CreateTreeMap(root, window.WebStatCanvas, 0);
            }
        }



        void CreateTreeMap(TreeNode node, Canvas currentCanvas, int level)
        {
            double w = currentCanvas.ActualWidth == 0 ? currentCanvas.Width : currentCanvas.ActualWidth;
            double h = currentCanvas.ActualHeight == 0 ? currentCanvas.Height : currentCanvas.ActualHeight;
            LayBuilder layBuilder = new LayBuilder(node, nodesOnLevel, w, h);
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
                    if (level == 0)
                    {
                        byte r = (byte)(rand.Next(minColorNum, maxColorNum));
                        byte g = (byte)(rand.Next(minColorNum, maxColorNum));
                        byte b = (byte)(rand.Next(minColorNum, maxColorNum));
                        color = Color.FromRgb(r, g, b);
                    }
                    else
                    {
                        color = ((SolidColorBrush)currentCanvas.Background).Color;
                    }
                    DockPanel dockPanel = new DockPanel();
                    TextBlock title = new TextBlock();
                    Color auxColor = color;
                    auxColor.R -= (byte)(10 * level+1);
                    auxColor.G -= (byte)(10 * level+1);
                    auxColor.B -= (byte)(10 * level+1);
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
                    if (node.TopRequests.Count != 0)
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
                    title.MouseEnter += (s, e) => { popup.IsOpen = true; };
                    title.MouseLeave += (s, e) => { popup.IsOpen = false; };
                    currentCanvas.Children.Add(dockPanel);
                    double canvasHeight = dockPanel.Height - title.Height;
                    if (canvasHeight <= 0 || dockPanel.Width <= 0)
                        continue;
                    canvas.Height = canvasHeight;
                    canvas.Width = dockPanel.Width;
                    title.Loaded += (s, e) => {
                        CreateTreeMap(rowElement.Node, canvas, level + 1);
                    };
                }
            }
        }
    }
}