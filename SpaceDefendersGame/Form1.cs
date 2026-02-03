using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceDefendersGame
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
            this.KeyPreview = true;
            gameTimer.Start();
            player.BringToFront();

            // Optional: Display starting score
            lblScore.Text = "Score: 0";
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
                goLeft = true;
            if (e.KeyCode == Keys.Right)
                goRight = true;
            if (e.KeyCode == Keys.Space && !shooting)
            {
                shooting = true;
                bullet = new PictureBox();
                bullet.Size = new Size(5, 20);
                bullet.BackColor = Color.Orange;
                bullet.Left = player.Left + player.Width / 2 - 2;
                bullet.Top = player.Top - 20;
                this.Controls.Add(bullet);


            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
                goLeft = false;
            if (e.KeyCode == Keys.Right)
                goRight = false;
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            // Move player
            if (goLeft && player.Left > 0)
                player.Left -= playSpd;
            if (goRight && player.Left < this.ClientSize.Width - player.Width)
                player.Left += playSpd;

            // Move bullet
            if (shooting)
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

                    if (bullet.Bounds.IntersectsWith(x.Bounds))
                    {
                        this.Controls.Remove(x);
                        this.Controls.Remove(bullet);
                        shooting = false;
                        score += 10;
                        lblScore.Text = "Score: " + score;
                    }

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
