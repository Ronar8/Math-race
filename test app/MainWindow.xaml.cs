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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Threading;
using System.Threading;

namespace math_race
{
    /// <summary>
    /// Logika interakcji dla klasy math_solving.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // inicjalizacja trzech stoperów
        DispatcherTimer gameTimer = new DispatcherTimer();
        DispatcherTimer stopwatch = new DispatcherTimer();
        DispatcherTimer countdown = new DispatcherTimer();

        ImageBrush playerSprite = new ImageBrush();
        ImageBrush background1_Sprite = new ImageBrush();
        ImageBrush background2_Sprite = new ImageBrush();
        ImageBrush obstacle1_Sprite = new ImageBrush();
        ImageBrush obstacle2_Sprite = new ImageBrush();

        Rect player_hitbox, ground_hitbox, obstacle1_hitbox, obstacle2_hitbox, platform_hitbox;

        // inicjalizacja zmiennej rand dla funkcji liczb losowych
        private static Random rand = new Random();

        // zmienne dotyczące modelu gracza
        bool jumping, is_touching_obstacle, difficulty_hard;
        int jump_force, player_speed, lifes, touched_obstacles, passed_obstacles, obstacle_number;
        double sprite_index;

        // zmienne dla stoperów 
        int timer, countdown_timer;

        // zmienne dla dynamicznej trudności gry
        double diff_scrolling, diff_player_sprite;

        // losowanie wysokości wystawania przeszkody
        int [] obstacle_height = {515, 520, 525};

        /// <summary>
        /// główne okno gry, wprowadzany jest parametr 'difficulty_hard' w wartości true lub false
        /// zależnie od wybranego poziomu trudności gry
        /// difficulty_hard == true --> wybrano poziom trudności liceum
        /// difficulty_hard == false --> wybrano poziom trudności szkoła podstawowa
        /// countdown_timer = 3 --> odliczanie trzech sekund przed startem gry
        /// </summary>
        /// <param name="difficulty_hard"></param>
        /// parametr 'difficulty_hard' ma wartość true lub false, zależnie od wybranego poziomu trudności gry
        /// difficulty_hard == true --> wybrano poziom trudności liceum
        /// difficulty_hard == false --> wybrano poziom trudności szkoła podstawowa
        public MainWindow(bool difficulty_hard)
        {
            InitializeComponent();

            MyCanvas.Focus();

            gameTimer.Tick += GameEngine;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);

            stopwatch.Interval = TimeSpan.FromSeconds(1);
            stopwatch.Tick += Stopwatch_Tick;

            countdown.Interval = TimeSpan.FromSeconds(1);
            countdown.Tick += Countdown_Tick;

            InitializeGameElements();

            // inicjalizacja odliczania 3 sekundowego
            countdown_timer = 3;
            countdown_3s.Content = "3";
            countdown.Start();

            this.difficulty_hard = difficulty_hard;
        }

        /// <summary>
        /// główna logika gry, aktualizowana co 20 milisekund
        /// int player_speed --> szybkość spadania modelu gracza
        /// bool jumping --> czy gracz obecnie jest podczas skoku
        /// double sprite_index --> animacja biegu gracza
        /// double diff_player_sprite --> dynamiczna zmiana szybkości biegu gracza
        /// double diff_scrolling --> dynamiczna zmiana prędkości przesuwania tła
        /// int obstacle_number --> która przeszkoda została dotknięta
        /// bool is_touching_obstacle --> czy model gracza dotknął przeszkody
        /// int touched_obstacles --> licznik dotkniętych przeszkód
        /// int passed_obstacles --> licznik ominiętych przeszkód
        /// int jump_force --> siła skoku
        /// </summary>
        private void GameEngine(object sender, EventArgs e)
        {
            // szybkość przesuwania tła
            Canvas.SetLeft(background, Canvas.GetLeft(background) - diff_scrolling);
            Canvas.SetLeft(background_2, Canvas.GetLeft(background_2) - diff_scrolling);

            // funkcja ciągłego tła
            if (Canvas.GetLeft(background) < -2048)
            {
                Canvas.SetLeft(background, Canvas.GetLeft(background_2) + background_2.Width - 1);
            }

            if (Canvas.GetLeft(background_2) < -2048)
            {
                Canvas.SetLeft(background_2, Canvas.GetLeft(background) + background.Width - 1);
            }

            // przesuwanie postaci gracza w dół ze stałą szybkością
            Canvas.SetTop(player, Canvas.GetTop(player) + player_speed);
            // przesuwanie przeszkód od prawej do lewej z dynamiczną prędkością
            Canvas.SetLeft(obstacle1, Canvas.GetLeft(obstacle1) - diff_scrolling - 4);
            Canvas.SetLeft(obstacle2, Canvas.GetLeft(obstacle2) - diff_scrolling - 4);
            Canvas.SetLeft(platform, Canvas.GetLeft(platform) - diff_scrolling - 4);

            // inicjalizacja hitboxów
            player_hitbox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width - 25, player.Height - 5);
            obstacle1_hitbox = new Rect(Canvas.GetLeft(obstacle1), Canvas.GetTop(obstacle1), obstacle1.Width - 45, obstacle1.Height - 30);
            obstacle2_hitbox = new Rect(Canvas.GetLeft(obstacle2), Canvas.GetTop(obstacle2), obstacle2.Width - 20, obstacle2.Height - 5);
            ground_hitbox = new Rect(Canvas.GetLeft(ground), Canvas.GetTop(ground), ground.Width, ground.Height);
            platform_hitbox = new Rect(Canvas.GetLeft(platform), Canvas.GetTop(platform), platform.Width, platform.Height);

            // logika 'lądowania' gracza na platformie lub ziemi
            if (player_hitbox.IntersectsWith(ground_hitbox) || player_hitbox.IntersectsWith(platform_hitbox))
            {
                player_speed = 0;

                if (player_hitbox.IntersectsWith(ground_hitbox))
                {
                    Canvas.SetTop(player, Canvas.GetTop(ground) - player.Height);
                }
                else if (player_hitbox.IntersectsWith(platform_hitbox))
                {
                    Canvas.SetTop(player, Canvas.GetTop(platform) - player.Height);
                }
                
                jumping = false;

                sprite_index += diff_player_sprite;

                // dynamiczny poziom trudności, przyspieszenie biegu gracza wraz z upływem czasu
                if (timer % 5 == 0 && timer != 0)
                {
                    diff_player_sprite += 0.0005;
                    diff_scrolling += 0.05;
                }

                // powtórzenie animacji biegu gracza
                if (sprite_index > 8)
                {
                    sprite_index = 1;
                }
                Sprite_Change(sprite_index);
            }

            // detekcja kolizji modelu gracza z przeszkodą
            if (player_hitbox.IntersectsWith(obstacle1_hitbox))
            {
                is_touching_obstacle = true;
                obstacle_number = 1;

            }
            else if (player_hitbox.IntersectsWith(obstacle2_hitbox))
            {
                is_touching_obstacle = true;
                obstacle_number = 2;
            }

            // logika gry, kiedy nastąpi kolizja
            if (is_touching_obstacle)
            {
                Game_Stop();
                is_touching_obstacle = false;
                touched_obstacles++;

                // otworzenie okna z danym problemem matematycznym
                Math_Solving math_window = new Math_Solving(difficulty_hard)
                {
                    Owner = this
                };
                math_window.ShowDialog();

                if (math_window.DialogResult == true)
                {
                    Change_Obstacle_Pos(obstacle_number);
                    Game_Start();
                }
                else
                {
                    lifes--;

                    // zmiana liczby żyć
                    if (lifes > 0)
                    {
                        Uri dynamic_hearts = new Uri("pack://application:,,,/lifes/hearts_" + lifes + ".png");
                        life_hearts.Source = new BitmapImage(dynamic_hearts);

                        Change_Obstacle_Pos(obstacle_number);
                        Game_Start();
                    }
                    else
                    {
                        life_hearts.Visibility = Visibility.Collapsed;

                        Game_Stop();

                        MessageBox.Show("Wyczerpano limit zyc, koniec gry");

                        // otworzenie okna podsumowania
                        Summary summary_window = new Summary(timer, passed_obstacles, touched_obstacles)
                        {
                            Owner = this
                        };
                        summary_window.ShowDialog();

                        this.Close();
                    }

                }
            }

            // limit długości skoku
            if (jumping == true)
            {
                player_speed = -10;
                jump_force -= 1;
            }
            else
            {
                player_speed = 12;
            }

            if (jump_force < 0)
            {
                jumping = false;
            }

            // powtórzenie przeszkód w oknie gry
            if (Canvas.GetLeft(obstacle1) < -50)
            {
                obstacle_number = 1;
                Change_Obstacle_Pos(obstacle_number);
                passed_obstacles++;
            }
            
            if (Canvas.GetLeft(obstacle2) < -50)
            {
                obstacle_number = 2;
                Change_Obstacle_Pos(obstacle_number);
                passed_obstacles++;
            }

            // powtórzenie dodatkowej platformy w oknie gry
            if (Canvas.GetLeft(platform) < -400)
            {
                Change_Platform_Position();
            }

        }

        /// <summary>
        /// zmiana lokalizacji przeszkód poza widoczne pole gry
        /// </summary>
        /// <param name="obstacle_number"></param>
        /// definiuje z którą przeszkodą gracz się zetknął
        private void Change_Obstacle_Pos(int obstacle_number)
        {
            if (obstacle_number == 1)
            {
                Canvas.SetLeft(obstacle1, 1100);

                Canvas.SetTop(obstacle1, obstacle_height[rand.Next(0, obstacle_height.Length)]);
            }
            else if (obstacle_number == 2)
            {
                Canvas.SetLeft(obstacle2, 1300);

                Canvas.SetTop(obstacle2, obstacle_height[rand.Next(0, obstacle_height.Length)]);
            }
        }

        /// <summary>
        /// zmiana lokalizacji platformy poza widoczne pole gry
        /// </summary>
        private void Change_Platform_Position()
        {
            Canvas.SetLeft(platform, 3100);
            Canvas.SetTop(platform, 470);
        }

        /// <summary>
        /// restart gry poprzez otworzenie nowego okna i zamknięcie obecnego
        /// </summary>
        private void Game_Restart_Btn(object sender, RoutedEventArgs e)
        {
            startup_window sW = new startup_window();
            sW.Show();
            this.Close();
        }

        /// <summary>
        /// otworzenie i zamknięcie menu podczas rozgrywki
        /// </summary>
        private void Menu_Btn(object sender, RoutedEventArgs e)
        {
            if (menu_Area.IsVisible)
            {
                Close_Menu();
                Game_Start();
            }
            else
            {
                Open_Menu();
                Game_Stop();
            }
        }

        /// <summary>
        /// zamknięcie menu podczas rozgrywki
        /// </summary>
        private void Close_Menu()
        {
            menu_Area.Visibility = Visibility.Collapsed;
            menu_Restart_Button.Visibility = Visibility.Collapsed;
            menu_Exit_Button.Visibility = Visibility.Collapsed;
            menu_Game_Exit_Button.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// otworzenie menu podczas rozgrywki
        /// </summary>
        private void Open_Menu()
        {
            menu_Area.Visibility = Visibility.Visible;
            menu_Restart_Button.Visibility = Visibility.Visible;
            menu_Exit_Button.Visibility = Visibility.Visible;
            menu_Game_Exit_Button.Visibility = Visibility.Visible;
        }


        private void Game_Exit_Btn(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Game_Menu_Exit_Btn(object sender, RoutedEventArgs e)
        {
            Close_Menu();
            Game_Start();
        }

        /// <summary>
        /// stoper liczący czas gry
        /// int timer --> licznik czasu gry
        /// </summary>
        private void Stopwatch_Tick(object sender, EventArgs e)
        {
            timer++;

            Time.Content = "Czas: " + timer + " s";
        }

        private void Countdown_Tick(object sender, EventArgs e)
        {
            if (countdown_timer > 0)
            {
                countdown_timer--;

                countdown_3s.Content = countdown_timer;
            }
            else
            {
                countdown.Stop();
                countdown_3s.Visibility = Visibility.Collapsed;
                StartGame();
            }
        }

        /// <summary>
        /// funkcja zatrzymania silnika gry
        /// </summary>
        private void Game_Stop()
        {
            gameTimer.Stop();
            stopwatch.Stop();
            countdown.Stop();
        }

        /// <summary>
        /// funkcja wystartowania silnika gry
        /// </summary>
        private void Game_Start()
        {
            if (countdown_timer > 0)
            {
                countdown.Start();
            }
            else
            {
                gameTimer.Start();
                stopwatch.Start();
            }
        }

        //two functions related to dynamic difficulty, not working properly
        //temporary fix applied
        /*private double time_difficulty_scrolling(double n)
        {
            if (timer % 5 == 0 && timer != 0)
            {
                return n + 0.1;
            }
            else
            {
                return n;
            }
        }

        private double time_difficulty_player(double m)
        {
            if (timer % 5 == 0 && timer != 0)
            {
                return m + 0.01;
            }
            else
            {
                return m;
            }
        }*/

        /// <summary>
        /// logika dla kliknięcia klawisza spacji, który odpowiada za skok modelu gracza
        /// </summary>
        private void Key_Up(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && jumping == false && Canvas.GetTop(player) > 260)
            {
                jumping = true;
                jump_force = 15;
                player_speed = -12;

                // obraz runner_02.gif odpowiada skokowi modelu gracza
                playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/runner_02.gif"));
            }
        }

        /// <summary>
        /// inicjalizacja elementów gry takich jak tło, początkowe położenie przeszkód, modelu gracza, liczba żyć
        /// </summary>
        private void InitializeGameElements()
        {
            background1_Sprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/background_1.png"));
            background.Fill = background1_Sprite;
            background2_Sprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/background_2.png"));
            background_2.Fill = background2_Sprite;

            Canvas.SetLeft(background, 0);
            Canvas.SetLeft(background_2, 2046);

            Canvas.SetLeft(player, 110);
            Canvas.SetTop(player, 460);

            Canvas.SetLeft(obstacle1, 950);
            Canvas.SetTop(obstacle1, 520);

            Canvas.SetLeft(platform, 900);
            Canvas.SetTop(platform, 470);

            ground.Visibility = Visibility.Collapsed;

            Sprite_Change(1);

            obstacle1_Sprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/obstacles/spike C.png"));
            obstacle1.Fill = obstacle1_Sprite;

            obstacle2_Sprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/obstacles/spike D.png"));
            obstacle2.Fill = obstacle2_Sprite;

            Uri default_hearts = new Uri("pack://application:,,,/lifes/hearts_3.png");
            life_hearts.Source = new BitmapImage(default_hearts);

            Close_Menu();
        }

        /// <summary>
        /// funkcja wystartowania gry
        /// </summary>
        private void StartGame()
        {
            jumping = false;

            jump_force = 20;
            player_speed = 5;

            timer = 0;
            lifes = 3;

            diff_scrolling = 8;
            diff_player_sprite = 0.5;

            passed_obstacles = 0;
            touched_obstacles = 0;

            sprite_index = 0;

            Game_Start();
        }

        /// <summary>
        /// funkcja zmiany modelu gracza w celu stworzenia animacji
        /// </summary>
        /// <param name="i"></param>
        /// parametr i odpowiada za przełączanie modeli gracza
        private void Sprite_Change(double i)
        {
            playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/runner_0" + Math.Ceiling(i) + ".gif"));
            player.Fill = playerSprite;
        }
    }
}
