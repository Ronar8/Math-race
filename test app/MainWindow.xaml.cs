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

namespace test_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        DispatcherTimer stopwatch = new DispatcherTimer();
        DispatcherTimer countdown = new DispatcherTimer();

        ImageBrush playerSprite = new ImageBrush();
        ImageBrush background1_Sprite = new ImageBrush();
        ImageBrush background2_Sprite = new ImageBrush();
        
        ImageBrush obstacle1_Sprite = new ImageBrush();
        ImageBrush obstacle2_Sprite = new ImageBrush();


        Rect player_hitbox, ground_hitbox, obstacle1_hitbox, obstacle2_hitbox;

        private static Random rand = new Random();

        //variables regarding player character
        bool jumping, is_touching_obstacle, difficulty_hard;
        int jump_force, player_speed, lifes, touched_obstacles, passed_obstacles, obstacle_number;
        double sprite_index;

        int timer, countdown_timer;

        double diff_scrolling, diff_player_sprite;

        //randomize obstacle height protruding from the ground
        int [] obstacle_height = {515, 520, 525};

        public MainWindow(bool difficulty_hard)
        {
            InitializeComponent();

            MyCanvas.Focus();

            gameTimer.Tick += GameEngine;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);

            stopwatch.Interval = TimeSpan.FromSeconds(1);
            stopwatch.Tick += stopwatch_Tick;

            countdown.Interval = TimeSpan.FromSeconds(1);
            countdown.Tick += countdown_Tick;

            InitializeGameElements();

            countdown_timer = 3;
            countdown_3s.Content = "3";
            countdown.Start();
            this.difficulty_hard = difficulty_hard;
        }

        private void GameEngine(object sender, EventArgs e)
        {

            //scrolling player_speed of background
            Canvas.SetLeft(background, Canvas.GetLeft(background) - diff_scrolling);
            Canvas.SetLeft(background_2, Canvas.GetLeft(background_2) - diff_scrolling);

            //parallax scrolling of background
            if (Canvas.GetLeft(background) < -2048)
            {
                Canvas.SetLeft(background, Canvas.GetLeft(background_2) + background_2.Width - 1);
            }

            if (Canvas.GetLeft(background_2) < -2048)
            {
                Canvas.SetLeft(background_2, Canvas.GetLeft(background) + background.Width - 1);
            }

            //land the player on the ground
            Canvas.SetTop(player, Canvas.GetTop(player) + player_speed);
            //move obstacle from right to left
            Canvas.SetLeft(obstacle1, Canvas.GetLeft(obstacle1) - diff_scrolling - 4);
            Canvas.SetLeft(obstacle2, Canvas.GetLeft(obstacle2) - diff_scrolling - 4);

            //setup hitboxes
            player_hitbox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width - 25, player.Height - 5);
            obstacle1_hitbox = new Rect(Canvas.GetLeft(obstacle1), Canvas.GetTop(obstacle1), obstacle1.Width, obstacle1.Height);
            obstacle2_hitbox = new Rect(Canvas.GetLeft(obstacle2), Canvas.GetTop(obstacle2), obstacle2.Width, obstacle2.Height);
            ground_hitbox = new Rect(Canvas.GetLeft(ground), Canvas.GetTop(ground), ground.Width, ground.Height);

            //logic when player lands on ground
            if (player_hitbox.IntersectsWith(ground_hitbox))
            {
                player_speed = 0;

                Canvas.SetTop(player, Canvas.GetTop(ground) - player.Height);

                jumping = false;

                sprite_index += diff_player_sprite;

                //dynamic difficulty, scrolling speed and player sprite change speed
                if (timer % 5 == 0 && timer != 0)
                {
                    diff_player_sprite += 0.001;
                    diff_scrolling += 0.1;
                }

                //repeat animation when running out of sprites (no more than 8)
                if (sprite_index > 8)
                {
                    sprite_index = 1;
                }

                Sprite_Change(sprite_index);
            }

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

            if (is_touching_obstacle)
            {
                game_Stop();

                is_touching_obstacle = false;

                touched_obstacles++;

                math_solving math_window = new math_solving(difficulty_hard);
                math_window.Owner = this;
                math_window.ShowDialog();

                if (math_window.DialogResult == true)
                {
                    change_obstacle_pos(obstacle_number);

                    game_Start();
                }
                else
                {
                    lifes--;

                    if (lifes > 0)
                    {
                        Uri dynamic_hearts = new Uri("pack://application:,,,/lifes/hearts_" + lifes + ".png");
                        life_hearts.Source = new BitmapImage(dynamic_hearts);

                        change_obstacle_pos(obstacle_number);

                        game_Start();
                    }
                    else
                    {
                        life_hearts.Visibility = Visibility.Collapsed;

                        game_Stop();

                        MessageBox.Show("Wyczerpano limit zyc, koniec gry");

                        summary summary_window = new summary(timer, passed_obstacles, touched_obstacles);
                        summary_window.Owner = this;
                        summary_window.ShowDialog();

                        this.Close();
                    }

                }
            }

            // limit jump range
            if (jumping == true)
            {
                player_speed = -10;
                jump_force -= 1;
            }
            else
            {
                player_speed = 12;
            }

            // if limit of jump height is reached, fall back down
            if (jump_force < 0)
            {
                jumping = false;
            }


            // repeat obstacle1
            if (Canvas.GetLeft(obstacle1) < -50)
            {
                obstacle_number = 1;
                change_obstacle_pos(obstacle_number);
                passed_obstacles++;
            }
            
            if (Canvas.GetLeft(obstacle2) < -50)
            {
                obstacle_number = 2;
                change_obstacle_pos(obstacle_number);
                passed_obstacles++;
            }

        }

        private void change_obstacle_pos(int obstacle_number)
        {
            if (obstacle_number == 1)
            {
                Canvas.SetLeft(obstacle1, 1100);

                Canvas.SetTop(obstacle1, obstacle_height[rand.Next(0, obstacle_height.Length)]);
            }
            else if (obstacle_number == 2)
            {
                Canvas.SetLeft(obstacle2, 1500);

                Canvas.SetTop(obstacle2, obstacle_height[rand.Next(0, obstacle_height.Length)]);
            }
        }

        private void game_Restart_Btn(object sender, RoutedEventArgs e)
        {
            MainWindow mW = new MainWindow(difficulty_hard);
            mW.Show();
            this.Close();
        }

        private void menu_Btn(object sender, RoutedEventArgs e)
        {
            if (menu_Area.IsVisible)
            {
                close_menu();

                game_Start();
            }
            else
            {
                open_menu();

                game_Stop();
            }
        }

        private void close_menu()
        {
            menu_Area.Visibility = Visibility.Collapsed;
            menu_Restart_Button.Visibility = Visibility.Collapsed;
            menu_Exit_Button.Visibility = Visibility.Collapsed;
            menu_Game_Exit_Button.Visibility = Visibility.Collapsed;
        }

        private void open_menu()
        {
            menu_Area.Visibility = Visibility.Visible;
            menu_Restart_Button.Visibility = Visibility.Visible;
            menu_Exit_Button.Visibility = Visibility.Visible;
            menu_Game_Exit_Button.Visibility = Visibility.Visible;
        }

        private void game_Exit_Btn(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void stopwatch_Tick(object sender, EventArgs e)
        {
            timer++;

            Time.Content = "Czas: " + timer + " s";
        }

        private void game_Menu_Exit(object sender, RoutedEventArgs e)
        {
            close_menu();

            game_Start();
        }

        private void game_Stop()
        {
            gameTimer.Stop();
            stopwatch.Stop();
            countdown.Stop();
        }

        private void game_Start()
        {
            gameTimer.Start();
            stopwatch.Start();
        }

        private void countdown_Tick(object sender, EventArgs e)
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
        private void Key_Up(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && jumping == false && Canvas.GetTop(player) > 260)
            {
                jumping = true;
                jump_force = 15;
                player_speed = -12;

                playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/runner_02.gif"));
            }
        }

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

            ground.Visibility = Visibility.Collapsed;

            Sprite_Change(1);

            obstacle1_Sprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/obstacles/spike C.png"));
            obstacle1.Fill = obstacle1_Sprite;

            obstacle2_Sprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/obstacles/spike D.png"));
            obstacle2.Fill = obstacle2_Sprite;

            Uri default_hearts = new Uri("pack://application:,,,/lifes/hearts_3.png");
            life_hearts.Source = new BitmapImage(default_hearts);

            close_menu();
        }

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

            game_Start();
        }

        private void Sprite_Change(double i)
        {
            playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/runner_0" + Math.Ceiling(i) + ".gif"));

            player.Fill = playerSprite;
        }
    }
}
