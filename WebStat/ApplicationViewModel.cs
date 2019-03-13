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
using System.Windows.Media.Effects;

namespace WebStat
{
    public class ApplicationViewModel
    {
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

        TextBlock getTextBlock(Color color,string name)
        {
            var textBlock= new TextBlock();
            textBlock.Background = new SolidColorBrush(color);
            textBlock.Text = name; 
            textBlock.TextAlignment = TextAlignment.Center;
            textBlock.FontSize = 16;
            textBlock.Height = 20;
            textBlock.Foreground = new SolidColorBrush(Colors.White);
            textBlock.SetValue(DockPanel.DockProperty, Dock.Top);
            return textBlock;
        }

        Popup getPopup(Color color,string title,List<Tuple<string,int>> requests)
        {
            Popup popup = new Popup();
            popup.Placement = PlacementMode.Mouse;
            DockPanel popupPanel = new DockPanel();
            TextBlock popupTitle = new TextBlock();
            popupTitle.Text = title;
            popup.Width = 250;
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
            listBoxNames.Background = new SolidColorBrush(color);
            listBoxNames.Foreground = new SolidColorBrush(Colors.White);
            listBoxNames.FontSize = 16;
            listBoxNames.HorizontalAlignment = HorizontalAlignment.Stretch;
            if (requests.Count != 0)
                listBoxNames.ItemsSource = (from r in requests select r.Item1 + " " + r.Item2).ToArray();
            listBoxNames.SetValue(DockPanel.DockProperty, Dock.Left);
            popupPanel.Children.Add(popupTitle);
            popupPanel.Children.Add(popupNotation);
            popupPanel.Children.Add(listBoxNames);
            popupPanel.Background = new SolidColorBrush(color);
            Border borderPopup = new Border();
            borderPopup.BorderThickness = new Thickness(1);
            borderPopup.BorderBrush = new SolidColorBrush(Colors.Coral);
            borderPopup.Child = popupPanel;
            popup.Child = borderPopup;
            return popup;
        }


        void changeNextBorder(FrameworkElement parent,Color color)
        {
            if (parent is Border)
            {
                var currentBorder = (Border)parent;
                currentBorder.BorderBrush = new SolidColorBrush(color);
            }
            else if (parent is Window)
            {
                return;
            }
            changeNextBorder((FrameworkElement)parent.Parent, color);
        }

        byte decreaseColor(byte value,int level)
        {
            byte subtrahend = (byte)(10 * level + 1);
            if (value - subtrahend >= 0)
                value -= subtrahend;
            return value;
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
                    Color auxColor = color;
                    auxColor.R = decreaseColor(auxColor.R, level);
                    auxColor.G = decreaseColor(auxColor.G, level);
                    auxColor.B = decreaseColor(auxColor.B, level);
                    DockPanel dockPanel = new DockPanel();
                    dockPanel.Width = elem.width;
                    dockPanel.Height = elem.height;
                    dockPanel.LastChildFill = true;
                    
                    TextBlock title = getTextBlock(auxColor, elem.Node.ShortName);
                    dockPanel.Children.Add(title);
                    dockPanel.LastChildFill = true;   
                    Canvas newCanvas = new Canvas();
                    newCanvas.Background = new SolidColorBrush(color);
                    Border border = new Border();
                    border.BorderThickness = new Thickness(4);
                    border.BorderBrush = new SolidColorBrush(Colors.Transparent);
                    border.Child = dockPanel;
                    border.Width = elem.width;
                    border.Height = elem.height;
                    border.SetValue(Canvas.LeftProperty, elem.left);
                    border.SetValue(Canvas.TopProperty, elem.top);
                    dockPanel.Children.Add(newCanvas);
                    Popup popup = getPopup(auxColor, elem.Node.ShortName, elem.Node.TopRequests);
                    title.MouseEnter += (s, e) => {
                        popup.IsOpen = true;
                        changeNextBorder(title,Colors.Red);
                    };
                    title.MouseLeave += (s, e) => {
                        popup.IsOpen = false;
                        changeNextBorder(title,Colors.Transparent);
                    };
                    currentCanvas.Children.Add(border);
                    newCanvas.Background = new SolidColorBrush(auxColor);
                    double canvasHeight = dockPanel.Height - title.Height;
                    if (canvasHeight > 0 && dockPanel.Width > 0)
                    {
                        newCanvas.Height = canvasHeight;
                        newCanvas.Width = dockPanel.Width;
                        title.Loaded += (s, e) =>
                        {
                            CreateTreeMap(elem.Node, newCanvas, level + 1);
                        };
                    }
                }
            }
        }
    }
}