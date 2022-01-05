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
    /// Interaction logic for summary.xaml
    /// </summary>
    public partial class summary : Window
    {
        int time_elapsed, obstacles_passed, obstacles_touched;

        private void game_End_Btn(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        public summary(int timer, int obstacles1, int obstacles2)
        {
            InitializeComponent();

            this.time_elapsed = timer;
            this.obstacles_passed = obstacles1;
            this.obstacles_touched = obstacles2;

            time.Text = time_elapsed.ToString() + " sekund";
            obstacles_p.Text = obstacles_passed.ToString();
            obstacles_t.Text = obstacles_touched.ToString();
        }
    }
}
