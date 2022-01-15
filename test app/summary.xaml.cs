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

namespace math_race
{
    /// <summary>
    /// Logika interakcji dla klasy summary.xaml
    /// </summary>
    public partial class Summary : Window
    {
        int time_elapsed, obstacles_passed, obstacles_touched;

        /// <summary>
        /// logika okna podsumowującego grę
        /// </summary>
        /// <param name="timer"></param>
        /// ile czasu upłynęło od wystartowania gry
        /// <param name="obstacles1"></param>
        /// ile przeszkód gracz ominął
        /// <param name="obstacles2"></param>
        /// ile przeszkód gracz dotknął
        public Summary(int timer, int obstacles1, int obstacles2)
        {
            InitializeComponent();

            this.time_elapsed = timer;
            this.obstacles_passed = obstacles1;
            this.obstacles_touched = obstacles2;

            time.Text = time_elapsed.ToString() + " sekund(y)";
            obstacles_p.Text = obstacles_passed.ToString();
            obstacles_t.Text = obstacles_touched.ToString();
        }

        /// <summary>
        /// logika dla przycisku 'Zakończ grę'
        /// </summary>
        private void Game_End_Btn(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
