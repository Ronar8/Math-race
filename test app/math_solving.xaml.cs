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

namespace test_app
{
    /// <summary>
    /// Logika interakcji dla klasy math_solving.xaml
    /// </summary>
    public partial class math_solving : Window
    {
        private static Random rnd = new Random();

        bool difficulty_hard;

        int addend1;
        int addend2;

        int multiplicant;
        int multiplier;

        int dividend;
        int divisor;
        int temp_quotient;

        double answer = 0;

        int timeLeft;

        DispatcherTimer math_timer = new DispatcherTimer();

        public math_solving(bool difficulty_hard)
        {
            InitializeComponent();

            math_timer.Interval = TimeSpan.FromSeconds(1);
            math_timer.Tick += timer_Tick;

            this.difficulty_hard = difficulty_hard;

            if (difficulty_hard)
            {
                math_generator_hard();
            }
            else
            {
                math_generator_easy();
            }



            switchh(addend1, addend2, multiplicant, multiplier, dividend, divisor);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                timeLeft--;
                timeLabel.Content = timeLeft + " sekund";
            }
            else
            {
                math_timer.Stop();
                timeLabel.Content = "Koniec czasu!";
                MessageBox.Show("Wyczerpano limit czasu, tracisz 1 zycie");
                math_answer.Text = answer.ToString();

                this.DialogResult = false;
            }
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            string input = math_answer.Text;

            int num_input = Int16.Parse(input);

            if (num_input == answer)
            {
                math_timer.Stop();
                this.DialogResult = true;
            }
            else
            {
                math_timer.Stop();
                this.DialogResult = false;
            }
        }

        public void math_generator_easy()
        {
            timeLeft = rnd.Next(13, 16);
            timeLabel.Content = "x seconds";
            math_timer.Start();

            addend1 = rnd.Next(1, 50);
            addend2 = rnd.Next(1, 50);

            multiplicant = rnd.Next(2, 10);
            multiplier = rnd.Next(2, 10);

            divisor = rnd.Next(2, 5);
            temp_quotient = rnd.Next(2, 5);
            dividend = divisor * temp_quotient;
        }


        public void math_generator_hard()
        {
            timeLeft = rnd.Next(7, 11);
            timeLabel.Content = "x seconds";
            math_timer.Start();

            addend1 = rnd.Next(1, 101);
            addend2 = rnd.Next(1, 101);

            multiplicant = rnd.Next(2, 15);
            multiplier = rnd.Next(2, 15);

            divisor = rnd.Next(2, 15);
            temp_quotient = rnd.Next(2, 15);
            dividend = divisor * temp_quotient;
        }

        public void switchh(int a_1, int a_2, int m_1, int m_2, int d_1, int d_2)
        {
            string operator_field;
            int math_Operator = rnd.Next(5);

            switch (math_Operator)
            {
                case 1:
                    operator_field = "+";
                    answer = addend1 + addend2;
                    break;
                case 2:
                    operator_field = "-";
                    answer = addend1 - addend2;
                    break;
                case 3:
                    operator_field = "*";
                    answer = multiplicant * multiplier;
                    break;
                case 4:
                    operator_field = "/";
                    answer = dividend / divisor;
                    break;
                default:
                    operator_field = "+";
                    answer = addend1 + addend2;
                    break;
            }

            if (operator_field == "+")
            {
                math_question.Content = "Podaj wynik działania: " + $"{addend1} {operator_field} {addend2}";
            }
            else if (operator_field == "*")
            {
                math_question.Content = "Podaj wynik działania: " + $"{multiplicant} * {multiplier}";
            }
            else
            {
                math_question.Content = "Podaj wynik działania: " + $"{dividend} / {divisor}";
            }
        }

        private void dialog_Cancel_Btn_Click(object sender, RoutedEventArgs e)
        {
            math_timer.Stop();
            this.DialogResult = false;
        }
    }
}
