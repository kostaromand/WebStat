using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WebStat
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        SettingsViewModel model;
        public SettingsWindow()
        {
            InitializeComponent();
            model = new SettingsViewModel(this);
        }

        private void ButtonPath_Click(object sender, RoutedEventArgs e)
        {
            model.getPath();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            model.StartBuild();
        }
    }
}
