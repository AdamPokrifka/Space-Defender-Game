using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceDefendersGameV3
{
    public partial class Form1 : Form
    {
        bool isMouseHold = false;
        bool isShooting = true;

        int score = 0;
        int highScore = 0;
        int alienSpd = 2;
        int difficultyLevel = 1;

        //milliseconds between shooting
        int fireCooldown = 30;

        Random rand = new Random();
        Timer cooldownTimer = new Timer();

        List<PictureBox> bullets = new List<PictureBox>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;

            //create cursor for cooldown
            Cursor.Clip = this.RectangleToScreen(this.ClientRectangle);
            cooldownTimer.Interval = fireCooldown;
            cooldownTimer.Tick += CooldownTimer_tick;

            //progress bar setup
            cooldownBar.Maximum = 100;
            cooldownBar.Value = 100;

            highScore = Properties.Settings.Default.HighScore;
            lblScore.Text = "Score" + score + " High Score:" + highScore;


            gameTimer.Start();
            player.BringToFront();

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //unlock cursor
            Cursor.Clip = Rectangle.Empty;

        }

        //mouse movement -> Keep player inside window/client
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            int leftMovement = e.X - (player.Width / 2);
            if (leftMovement < 0)
                leftMovement = 0;
            if (leftMovement > this.ClientSize.Width - player.Width)
                leftMovement = this.ClientSize.Width - player.Width;
            player.Left = leftMovement;

        }

        //allows mouse shooting -> hold to rapid fire
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                isMouseHold = true;

        }


        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                isMouseHold = false;

        }

        private void CooldownTimer_tick(object sender, EventArgs e)
        {
            if(cooldownBar.Value < 100)
            {
                cooldownBar.Value += 10;
            }
            else
            {
                cooldownTimer.Stop();
                isShooting = true;
                this.Cursor = Cursors.Default;
            }

        }

        private void ShootBullet()
        {
            if(!isShooting)
                return;


            isShooting = false;
            cooldownBar.Value = 0;
            this.Cursor = Cursors.WaitCursor;
            cooldownTimer.Start();

            //Bullet Creation
            PictureBox bullet = new PictureBox();
            bullet.Size = new Size(5, 20);
            bullet.BackColor = Color.LightGreen;
            bullet.Left = player.Left + (player.Width / 2) - (bullet.Width / 2);
            bullet.Top = player.Top - bullet.Height;
            bullet.Tag = "bullet";
            this.Controls.Add(bullet);
            bullets.Add(bullet);
            bullet.BringToFront();


        }

        private void GameTimer_tick(object sender, EventArgs e)
        {
            //Auto fire while held
            if(isMouseHold && isShooting)
                ShootBullet();

            //move bullet
            foreach(PictureBox pb in bullets.ToList())
            {
                pb.Top -= 20;

                if(pb.Top < 0)
                {
                    bullets.Remove(pb);
                    this.Controls.Remove(pb);
                }

            }    

            //Movement of aliens and hitting aliens
            foreach(PictureBox alien in this.Controls.OfType<PictureBox>())
            {
                if((string)alien.Tag == "alien")
                {
                    alien.Top += alienSpd;

                    if(alien.Bounds.IntersectsWith(player.Bounds))
                    {
                        GameOver();
                        return;
                    }

                    foreach (PictureBox pb in bullets.ToList())
                    {
                        if (pb.Bounds.IntersectsWith(alien.Bounds))
                        {
                            //resets aliens instead of removing them
                            alien.Top = rand.Next(0 , 60);
                            alien.Left = rand.Next(20 , this.ClientSize.Width - alien.Width - 20);

                            this.Controls.Remove(pb);
                            bullets.Remove(pb);

                            score += 10;
                            lblScore.Text = "Score:" + score + " High Score:" + highScore;
                        }

                    }

                    //respawn
                    if(alien.Top > this.ClientSize.Height)
                    {
                        alien.Top = rand.Next(0, 60);
                        alien.Left = rand.Next(20, this.ClientSize.Width - alien.Width - 20);
                    }

                }

            }

            //difficulty
            if(score % 50 == 0 && score != 0) 
                alienSpd = 2 + (score / 50);

        }

        private void GameOver()
        {
            gameTimer.Stop();
            cooldownTimer.Stop();

            if(score > highScore)
            {
                highScore = score;
                Properties.Settings.Default.HighScore = highScore;
                Properties.Settings.Default.Save();
            }

            DialogResult results = MessageBox.Show("Game Over!\n\nScore: " + score + "\nHigh Score: " + highScore + "\n\nPlay Again?" + MessageBoxButtons.YesNo + MessageBoxIcon.Information);
            
             if (results == DialogResult.Yes)
                 GameRestart();
             else
                 this.Close();
            
        }

        private void GameRestart()
        {
            foreach(Control c in this.Controls.OfType<PictureBox>().ToList())
            {
                if((string)c.Tag == "bullet")
                    this.Controls.Remove(c);
            }
            bullets.Clear();

            //reset variables
            score = 0;
            alienSpd = 2;
            isShooting = true;
            cooldownBar.Value = 100;
            lblScore.Text = "Score: " + score + " High Score: " + highScore;

            //reset aliens
            foreach (PictureBox alien in this.Controls.OfType<PictureBox>().ToList())
            {
                
                if((string)alien.Tag == "alien")
                {
                    alien.Top = rand.Next(0, 60);
                    alien.Left = rand.Next(20, this.ClientSize.Width - alien.Width - 20);

                }
            }
        }

        




    }
}
