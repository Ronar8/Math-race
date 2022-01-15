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

        /// <summary>
        /// logika dla okna startowego
        /// int timeLeft_popup --> jak długo powiadomienie o zmianie trudności widnieje w oknie
        /// </summary>
        public startup_window()
        {
            InitializeComponent();

            popup_timer.Interval = TimeSpan.FromSeconds(2);
            popup_timer.Tick += Popup_Timer_Tick;

            timeLeft_popup = 2;
        }

        /// <summary>
        /// stoper określający czas widnienia powiadomienia o zmianie trudności w oknie startowym
        /// po upływie określonego czasu, powiadomienie znika
        /// </summary>
        private void Popup_Timer_Tick(object sender, EventArgs e)
        {
            if (timeLeft_popup > 0)
            {
                timeLeft_popup--;
            }
            else
            {
                popup_timer.Stop();
                difficulty_Popup.IsOpen = false;
            }
        }
        
        /// <summary>
        /// logika dla przycisku 'Start gry'
        /// </summary>
        private void Game_Start_Btn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(difficulty_hard);
            mainWindow.Show();
            this.Close();
        }

        /// <summary>
        /// logika dla przycisku 'Wyjdź z gry'
        /// </summary>
        private void Game_Exit_Btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// logika dla przycisku 'Zmień poziom trudności'
        /// </summary>
        private void Game_Difficulty_Btn_Click(object sender, RoutedEventArgs e)
        {
            popup_timer.Start();

            difficulty_Popup.IsOpen = true;

            if (difficulty_hard == false)
            {
                difficulty_status.Content = "Aktualny poziom trudności: LICEUM";
                difficulty_Popup_txtBox.Text = "Zmieniono poziom trudności na: LICEUM";
                difficulty_hard = true;
            }
            else
            {
                difficulty_status.Content = "Aktualny poziom trudności: SZKOLA PODSTAWOWA";
                difficulty_Popup_txtBox.Text = "Zmieniono poziom trudności na: SZKOLA PODSTAWOWA";
                difficulty_hard = false;
            }
        }
    }
}
