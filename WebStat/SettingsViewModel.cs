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
using System.Windows.Input;


namespace WebStat
{
    public class SettingsViewModel
    {
        SettingsWindow window;
        string filePath="";
        int nodesOnLevel=5;
        int topRequestCount=5;
        int levelCount = 5;
        TreeBuilder builder;
        List<ContentControl> levels;
        public SettingsViewModel(SettingsWindow window)
        {
            this.window = window;
            levels = new List<ContentControl>();
            levels.Add(window.GroupBoxLevel1);
            levels.Add(window.GroupBoxLevel2);
            levels.Add(window.GroupBoxLevel3);
            levels.Add(window.GroupBoxLevel4);
            levels.Add(window.GroupBoxLevel5);
        }

        public void getPath()
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePath = ofd.FileName;
                window.TextBlockPath.Text = filePath;
            }
        }
        void getTreeParameters()
        {
            int temp = 0;
            bool checkOnNum = int.TryParse(window.NodesOnLevelTextBox.Text, out temp);
            if (checkOnNum && temp > 0)
            {
                if (temp > 10)
                    nodesOnLevel = 10;
                nodesOnLevel = temp;
            }
            checkOnNum = int.TryParse(window.TopRequestCountTextBox.Text, out temp);
            if (checkOnNum && temp > 0)
            {
                if (temp > 10)
                    topRequestCount = 10;
                topRequestCount = temp;
            }
            checkOnNum = int.TryParse(window.LevelsTextBox.Text, out temp);
            if (checkOnNum && temp > 0)
            {
                if (temp > 5)
                    levelCount = 5;
                levelCount = temp;
            }

        }
        public void StartBuild()
        {
            if(filePath=="")
            {
                MessageBox.Show("Путь не указан");
                return;
            }
            try
            {
                getTreeParameters();
                TreeInfo info = new TreeInfo();
                for (int i = 0; i < levelCount; i++)
                {
                    StackPanel panel = (StackPanel)levels[i].Content;
                    var levelType = (LevelType)((ComboBox)panel.Children[1]).SelectedItem;
                    var popupType = (PopupLevelType)((ComboBox)panel.Children[3]).SelectedItem;
                    NodeLevelInfo levelInfo = new NodeLevelInfo(levelType, popupType, i);
                    info.AddNewLevelInfo(levelInfo);
                }
                info.nodesOnLevel = nodesOnLevel;
                info.topRequestCount = topRequestCount;
                info.levelCount = levelCount;
                builder = new TreeBuilder(topRequestCount, new CSVDataReader(filePath), info);
                TreeNode root = builder.GetTree();
                MainWindow mainWindow = new MainWindow(root, info);
                mainWindow.Show();
                window.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}