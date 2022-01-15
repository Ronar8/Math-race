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
    /// Logika interakcji dla klasy math_solving.xaml
    /// </summary>
    public partial class Math_Solving : Window
    {
        DispatcherTimer math_timer = new DispatcherTimer();

        // inicjalizacja zmiennej rand dla funkcji liczb losowych
        private static Random rnd = new Random();

        bool difficulty_hard;

        // zmienne dla funkcji dodawania
        int addend1, addend2;

        // zmienne dla funkcji mnożenia
        int multiplicant, multiplier;

        // zmienne dla funkcji dzielenia
        int dividend, divisor, temp_quotient;

        short num_input;
        double answer = 0;

        int timeLeft;

        /// <summary>
        /// logika dla okna z automatycznie wygenerowanym problemem matematycznym
        /// </summary>
        /// <param name="difficulty_hard"></param>
        /// parametr difficulty_hard spełnia tą samą funkcję co w głównym oknie gry (MainWindow.xaml)
        public Math_Solving(bool difficulty_hard)
        {
            InitializeComponent();

            math_timer.Interval = TimeSpan.FromSeconds(1);
            math_timer.Tick += Timer_Tick;

            if (difficulty_hard)
            {
                Math_Generator_Hard();
            }
            else
            {
                Math_Generator_Easy();
            }

            Math_Question_Generator(addend1, addend2, multiplicant, multiplier, dividend, divisor);

            this.difficulty_hard = difficulty_hard;
        }

        /// <summary>
        /// funkcja odliczania pozostałego czasu na rozwiązanie problemu matematycznego
        /// wyczerpanie limitu czasu skutkuje automatyczną utratą życia i powrotem do głównej gry
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
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

        /// <summary>
        /// funkcja dla kliknięcia przycisku 'Sprawdź', który sprawdza poprawność odpowiedzi
        /// </summary>
        private void Btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            bool parse_succesful = Int16.TryParse(math_answer.Text, out num_input);

            if (parse_succesful)
            {
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
            else
            {
                math_timer.Stop();
                this.DialogResult = false;
            }

        }

        /// <summary>
        /// generowanie liczb do budowy problemów matematycznych
        /// wariant 'easy' jest dostostowany do poziomu szkoły podstawowej
        /// wprowadzono dodatkową zmienną w operacji dzielenia aby uniknąć ułamków
        /// </summary>
        public void Math_Generator_Easy()
        {
            timeLeft = rnd.Next(13, 16);
            timeLabel.Content = "x seconds";
            math_timer.Start();

            addend1 = rnd.Next(1, 25);
            addend2 = rnd.Next(1, 25);

            multiplicant = rnd.Next(2, 10);
            multiplier = rnd.Next(2, 10);

            divisor = rnd.Next(2, 5);
            temp_quotient = rnd.Next(2, 5);
            dividend = divisor * temp_quotient;
        }

        /// <summary>
        /// generowanie liczb do budowy problemów matematycznych
        /// wariant 'hard' jest dostostowany do poziomu liceum
        /// wprowadzono dodatkową zmienną w operacji dzielenia aby uniknąć ułamków
        /// </summary>
        public void Math_Generator_Hard()
        {
            timeLeft = rnd.Next(7, 11);
            timeLabel.Content = "x seconds";
            math_timer.Start();

            addend1 = rnd.Next(1, 76);
            addend2 = rnd.Next(1, 76);

            multiplicant = rnd.Next(2, 15);
            multiplier = rnd.Next(2, 15);

            divisor = rnd.Next(2, 15);
            temp_quotient = rnd.Next(2, 15);
            dividend = divisor * temp_quotient;
        }

        /// <summary>
        /// zadaniem tej funkcji jest wykorzystanie wygenerowanych liczb w celu zbudowania problemu matematycznego
        /// dostępne są 4 operacje - dodawanie, odejmowanie, dzielenie, mnożenie
        /// </summary>
        /// <param name="a_1"></param>
        /// pierwszy składnik dodawania
        /// <param name="a_2"></param>
        /// drugi składnik dodawania
        /// <param name="m_1"></param>
        /// pierwszy czynnik mnożenia
        /// <param name="m_2"></param>
        /// drugi czynnik mnożenia
        /// <param name="d_1"></param>
        /// pierwszy operand dzielenia
        /// <param name="d_2"></param>
        /// drugi operand dzielenia
        public void Math_Question_Generator(int a_1, int a_2, int m_1, int m_2, int d_1, int d_2)
        {
            string operator_field;
            int math_Operator = rnd.Next(1, 5);

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
            else if (operator_field == "-")
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

        /// <summary>
        /// logika dla przycisku 'Anuluj'
        /// </summary>
        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            math_timer.Stop();
            this.DialogResult = false;
        }
    }
}
