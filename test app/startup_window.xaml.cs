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
using System.Windows.Threading;

namespace math_race
{
    /// <summary>
    /// Logika interakcji dla klasy startup_window.xaml
    /// </summary>
    public partial class startup_window : Window
    {
        DispatcherTimer popup_timer = new DispatcherTimer();

        bool difficulty_hard = false;

        int timeLeft_popup;
        public startup_window()
        {
            InitializeComponent();

            popup_timer.Interval = TimeSpan.FromSeconds(2);
            popup_timer.Tick += popup_timer_tick;

            timeLeft_popup = 2;
        }

        private void popup_timer_tick(object sender, EventArgs e)
        {
            if (timeLeft_popup > 0)
            {
                timeLeft_popup--;
            }
            else
            {
                popup_timer.Stop();
                difficulty_Change.IsOpen = false;
            }
        }

        private void game_Start_Btn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(difficulty_hard);
            mainWindow.Show();
            this.Close();
        }

        private void game_Exit_Btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void game_Difficulty_Btn_Click(object sender, RoutedEventArgs e)
        {
            popup_timer.Start();

            difficulty_Change.IsOpen = true;

            if (difficulty_hard == false)
            {
                difficulty_status.Content = "Aktualny poziom trudności: LICEUM";
                difficulty_Change_txtBox.Text = "Zmieniono poziom trudności na: LICEUM";
                difficulty_hard = true;
            }
            else
            {
                difficulty_status.Content = "Aktualny poziom trudności: SZKOLA PODSTAWOWA";
                difficulty_Change_txtBox.Text = "Zmieniono poziom trudności na: SZKOLA PODSTAWOWA";
                difficulty_hard = false;
            }
            
            
        }
    }
}
