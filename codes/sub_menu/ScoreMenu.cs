using RhythmGame.codes.custom_button;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace RhythmGame
{
    public partial class ScoreMenu : UserControl
    {
        TextBox usernameTxtBx;
        Label scoreLbl;


        public ScoreMenu(GameForm form)
        {
            
            this.Size = new Size(1000, 700);
            this.BackColor = Color.FromArgb(200, 0, 0, 0);
            this.DoubleBuffered = true;
            this.Visible = false;
            this.Enabled = false;

            scoreLbl = new Label
            {
                Text = $"Score: ",
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(280, 50),
                Font = new Font("Ariel", 34, FontStyle.Bold),
                Location = new Point(350, 150),
                BackColor = Color.Transparent,
                ForeColor = Color.White
            };
            this.Controls.Add(scoreLbl);

            this.Controls.Add(new Label
            {
                Text = "Enter Username",
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(280, 50),
                Font = new Font("Ariel", 18, FontStyle.Bold),
                Location = new Point(350, 250),
                BackColor = Color.Transparent,
                ForeColor = Color.White
            });

            usernameTxtBx = new TextBox
            {
                TextAlign = HorizontalAlignment.Center,
                Size = new Size(200, 50),
                Font = new Font("Ariel", 18, FontStyle.Bold),
                Location = new Point(390, 300),
            };
            CustomButton exitBtn = new CustomButton("quit", new Size(180, 40), false);
            exitBtn.Location = new Point(400, 450);
            exitBtn.Click += (sender, e) => ExitLevel(form);

            CustomButton restartBtn = new CustomButton("playagain", new Size(180, 40), false);
            restartBtn.Location = new Point(400, 400);
            restartBtn.Click += (sender, e) => form.GamePauseMenu.RestartLevel(form);

            this.Controls.Add(exitBtn);
            this.Controls.Add(restartBtn);
            this.Controls.Add(usernameTxtBx);
        }
        public void ExitLevel(GameForm form)
        {
            if(usernameTxtBx.Text != "")
            {
                if (!File.Exists($"{form.LevelData.LevelDirectory}\\{form.LevelData.LevelDifficulty}Leaderboard.txt"))
                    File.CreateText($"{form.LevelData.LevelDirectory}\\{form.LevelData.LevelDifficulty}Leaderboard.txt").Close();
                using (FileStream fs = new FileStream($"{form.LevelData.LevelDirectory}\\{form.LevelData.LevelDifficulty}Leaderboard.txt", FileMode.Append))
                {
                    StreamWriter writer = new StreamWriter(fs);
                    writer.WriteLine($"{form.Score}:{usernameTxtBx.Text}");
                    writer.Flush();
                    writer.Close();
                    fs.Close();
                }
                form.MainMenu.Location = form.Location;
                form.MainMenu.Show();
                this.Hide();
                form.Hide();        
            }
        }

        public void OpenScoreMenu(GameForm form)
        {
            scoreLbl.Text = $"Score: {form.Score}";
            this.Visible = true;
            this.Enabled = true;
            this.Focus();
            GameForm.GamePaused = true;
            form.GameTimer.Enabled = false;
            form.Program_StpWtch.Stop();
            form.MusicPlayer.Pause();
        }
    }
}
