using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
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
        double borderThickness = 4;
        Color borderColor = new Color() { A = 255, R = 74, G = 179, B = 198 };
        NumberFormatInfo numFormat = new CultureInfo("en-US", false).NumberFormat;
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
                CreateTreeMap(root, window.WebStatCanvas, 1);
            }
        }

        TextBlock getTextBlock(Color color,string name)
        {
            var textBlock= new TextBlock();
            textBlock.Background = new SolidColorBrush(color);
            textBlock.Text = name; 
            textBlock.TextAlignment = TextAlignment.Center;
            textBlock.FontSize = 16;
            textBlock.Height = 25;
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
            popup.Width = 400;
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
            DataGrid dataGrid = new DataGrid();
            var displayRequests = (from r in requests select new {
                Request = r.Item1,
                Value = r.Item2.ToString("N", numFormat)
            }).ToList();
            dataGrid.ItemsSource = displayRequests;
            dataGrid.ColumnWidth = new DataGridLength(50, DataGridLengthUnitType.Star);
            popupPanel.Children.Add(popupTitle);
            popupPanel.Children.Add(popupNotation);
            popupPanel.Children.Add(dataGrid);
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

        Color decreaseColor(Color color,int level,int degree)
        {
            byte subtrahend = (byte)(degree * level + 1);
            if (color.R - subtrahend >= 0)
                color.R -= subtrahend;
            if (color.G - subtrahend >= 0)
                color.G -= subtrahend;
            if (color.B - subtrahend >= 0)
                color.B -= subtrahend;
            return color;
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
                    if (level == 1)
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
                    Color auxColor = decreaseColor(color, level,4);
                    DockPanel dockPanel = new DockPanel();
                    dockPanel.Width = elem.Width - borderThickness*2;
                    dockPanel.Height = elem.Height - borderThickness * 2;
                    dockPanel.LastChildFill = true;    
                    TextBlock title = getTextBlock(decreaseColor(auxColor, level, 6), elem.Node.ShortName);
                    if (elem.Width < 50)
                    {
                        title.Text = "";
                        title.Height = elem.Height;
                        title.Background = new SolidColorBrush(auxColor);
                    }
                    dockPanel.Children.Add(title);
                    dockPanel.LastChildFill = true;   
                    Canvas newCanvas = new Canvas();
                    newCanvas.Background = new SolidColorBrush(auxColor);
                    Border border = new Border();
                    border.BorderThickness = new Thickness(borderThickness);
                    border.BorderBrush = new SolidColorBrush(Colors.Transparent);
                    border.Child = dockPanel;
                    border.Width = elem.Width;
                    border.Height = elem.Height;
                    border.SetValue(Canvas.LeftProperty, elem.Left);
                    border.SetValue(Canvas.TopProperty, elem.Top);
                    DockPanel.SetDock(newCanvas, Dock.Top);
                    dockPanel.Children.Add(newCanvas);
                    dockPanel.SetValue(Panel.ZIndexProperty, level);
                    Popup popup = getPopup(auxColor, elem.Node.ShortName, elem.Node.TopRequests);
                    title.MouseEnter += (s, e) => {
                        popup.IsOpen = true;
                        border.BorderBrush = new SolidColorBrush(borderColor);
                    };
                    title.MouseLeave += (s, e) => {
                        popup.IsOpen = false;
                        border.BorderBrush = new SolidColorBrush(Colors.Transparent);
                    };
                    currentCanvas.Children.Add(border);
                    double canvasHeight = dockPanel.Height - title.Height;
                    if (canvasHeight > borderThickness*2 && dockPanel.Width > borderThickness * 2)
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