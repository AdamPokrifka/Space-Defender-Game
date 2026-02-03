using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceDefendersGameV2
{
    public partial class Form1 : Form
    {
        int playSpd = 10;
        bool goLeft;
        bool goRight;
        bool shooting;
        int score = 0;
        int shootingSpeed = 20;
        PictureBox bullet = new PictureBox();
        Random rand = new Random();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            this.MouseMove += Form1_MouseMove;
            this.MouseClick += Form1_MouseClick;
            this.KeyPreview = true;
            gameTimer.Start();
            player.BringToFront();

            // Optional: Display starting score
            lblScore.Text = "Score: 0";
        }


        //This moves the player left to right and right to left
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            player.Left = e.X - (player.Width / 2);
        }

        //Shoot the bullet using left mouse click
        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left && !shooting)
            {
                shooting = true;
                bullet.Size = new Size(5, 20);
                bullet.BackColor = Color.Green;
                bullet.Left = player.Left + (player.Width / 2 - 2);
                bullet.Top = player.Top - 20;
                this.Controls.Add(bullet);
                bullet.BringToFront();
            }

        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            // Move bullet
            if (shooting & bullet != null)
            {
                bullet.Top -= shootingSpeed;
                if (bullet.Top < 0)
                {
                    shooting = false;
                    this.Controls.Remove(bullet);
                }
            }

            // Check collision with aliens
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "alien")
                {
                    x.Top += 1;

                    //Bullet hits alien
                    if (shooting && bullet != null && bullet.Bounds.IntersectsWith(x.Bounds))
                    {
                        this.Controls.Remove(x);
                        this.Controls.Remove(bullet);
                        shooting = false;
                        score += 10;
                        lblScore.Text = "Score: " + score;
                    }


                    //Alien hits player
                    if (x.Bounds.IntersectsWith(player.Bounds))
                    {
                        gameTimer.Stop();
                        lblScore.Text = "Game Over!";
                    }
                }
            }
        }


    }
}

