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

namespace test_app
{
    /// <summary>
    /// Logika interakcji dla klasy startup_window.xaml
    /// </summary>
    public partial class startup_window : Window
    {
        public startup_window()
        {
            InitializeComponent();
        }

        private void game_Start_Btn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void game_Exit_Btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
