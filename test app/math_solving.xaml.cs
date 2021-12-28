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

        int addend1;
        int addend2;

        double answer = 0;

        int timeLeft;

        DispatcherTimer math_timer = new DispatcherTimer();

        public math_solving()
        {
            InitializeComponent();

            math_timer.Interval = TimeSpan.FromSeconds(1);
            math_timer.Tick += timer_Tick;

            math_generator_easy();

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
                this.DialogResult = true;
                math_timer.Stop();
            }
            else
            {
                this.DialogResult = false;
                math_timer.Stop();
            }
        }

        public void math_generator_easy()
        {
            timeLeft = rnd.Next(7, 11);
            timeLabel.Content = " 30 seconds";
            math_timer.Start();

            addend1 = rnd.Next(1, 50);
            addend2 = rnd.Next(1, 50);

            string string1 = addend1.ToString();
            string string2 = addend2.ToString();
            string merged_string = string1 + string2;

            string operator_field = "";


            int math_Operator = rnd.Next(5);
            
            switch(math_Operator)
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
                    answer = addend1 * addend2;
                    break;
                case 4:
                    operator_field = "/";
                    answer = addend1 / addend2;
                    break;
                default:
                    operator_field = "+";
                    answer = addend1 + addend2;
                    break;
            }




            math_question.Content = "What is the result: " + string1 + operator_field + string2;

        }

    }
}
