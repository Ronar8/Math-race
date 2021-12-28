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
        ImageBrush obstacle3_Sprite = new ImageBrush();
        ImageBrush obstacle4_Sprite = new ImageBrush();

        Rect player_hitbox, ground_hitbox, obstacle1_hitbox;

        Rect obstacle2_hitbox;
        Rect obstacle3_hitbox;
        Rect obstacle4_hitbox;

        private static Random rand = new Random();

        //variables regarding player character
        bool jumping;
        int jump_force, player_speed, lifes, touched_obstacles;
        double sprite_index;

        int timer, countdown_timer;

        double diff_scrolling, diff_player_speed;

        bool stop_game;

        //randomize obstacle height protruding from the ground
        int [] obstacle_height = {500, 505, 510, 515, 520};


        public MainWindow()
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
            Canvas.SetLeft(obstacle1, Canvas.GetLeft(obstacle1) - 12);

            //setup hitboxes
            player_hitbox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width - 25, player.Height - 5);
            obstacle1_hitbox = new Rect(Canvas.GetLeft(obstacle1), Canvas.GetTop(obstacle1), obstacle1.Width, obstacle1.Height);
            ground_hitbox = new Rect(Canvas.GetLeft(ground), Canvas.GetTop(ground), ground.Width, ground.Height);

            //logic when player lands on ground
            if (player_hitbox.IntersectsWith(ground_hitbox))
            {
                player_speed = 0;

                Canvas.SetTop(player, Canvas.GetTop(ground) - player.Height);

                jumping = false;

                sprite_index += diff_player_speed;

                //dynamic difficulty, scrolling speed and player sprite change speed
                if (timer % 5 == 0 && timer != 0)
                {
                    diff_player_speed += 0.001;
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
                stop_game = true;

                gameTimer.Stop();
                stopwatch.Stop();

                touched_obstacles++;

                math_solving math_window = new math_solving();
                math_window.ShowDialog();

                if (math_window.DialogResult == true)
                {
                    change_obstacle1_pos();

                    stop_game = false;

                    gameTimer.Start();
                    stopwatch.Start();
                }
                else
                {
                    lifes--;

                    if (lifes > 0)
                    {
                        Uri dynamic_hearts = new Uri("pack://application:,,,/lifes/hearts_" + lifes + ".png");
                        life_hearts.Source = new BitmapImage(dynamic_hearts);

                        change_obstacle1_pos();

                        stop_game = false;

                        gameTimer.Start();
                        stopwatch.Start();
                    }
                    else
                    {
                        life_hearts.Visibility = Visibility.Collapsed;

                        stop_game = true;

                        gameTimer.Stop();
                        stopwatch.Stop();

                        MessageBox.Show("Wyczerpano limit zyc, koniec gry");

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
                change_obstacle1_pos();
            }

        }

        private void change_obstacle1_pos()
        {
            Canvas.SetLeft(obstacle1, 950);

            Canvas.SetTop(obstacle1, obstacle_height[rand.Next(0, obstacle_height.Length)]);
        }

        private void stopwatch_Tick(object sender, EventArgs e)
        {
            timer++;

            Time.Content = "Czas: " + timer + " s";
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

        private void Key_Down(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.R && stop_game == true)
            {
                StartGame();
                stopwatch.Start();
            }
        }

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
            Canvas.SetTop(obstacle1, 510);

            Sprite_Change(1);

            obstacle1_Sprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/obstacle.png"));
            obstacle1.Fill = obstacle1_Sprite;

            Uri default_hearts = new Uri("pack://application:,,,/lifes/hearts_3.png");
            life_hearts.Source = new BitmapImage(default_hearts);
        }

        private void StartGame()
        {
            jumping = false;
            stop_game = false;

            jump_force = 20;
            player_speed = 5;

            timer = 0;
            lifes = 3;

            diff_scrolling = 8;
            diff_player_speed = 0.5;

            sprite_index = 0;

            gameTimer.Start();
            stopwatch.Start();
        }

        private void Sprite_Change(double i)
        {
            playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/runner_0" + Math.Ceiling(i) + ".gif"));

            player.Fill = playerSprite;
        }
    }
}
