using RhythmGame.codes.custom_button;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RhythmGame
{
    public partial class PauseMenu : UserControl
    {
        CustomButton resumeBtn;
        CustomButton restartBtn;
        CustomButton exitBtn;


        public PauseMenu(GameForm form)
        {
            this.Size = new Size(1000, 700);
            this.Visible = false;
            this.Enabled = false;
            this.DoubleBuffered = true;
            this.BackColor = Color.FromArgb(200, 0, 0, 0);

            resumeBtn = new CustomButton("resume", new Size(200, 50), false);
            restartBtn = new CustomButton("restart", new Size(200, 50), false);
            exitBtn = new CustomButton("quit", new Size(200, 50), false);
            

            resumeBtn.Location = new Point(this.Size.Width / 2 - 109, this.Size.Height / 2 - 200);
            restartBtn.Location = new Point(this.Size.Width / 2 - 109, this.Size.Height / 2 - 140);
            exitBtn.Location = new Point(this.Size.Width / 2 - 109, this.Size.Height / 2 - 80);

            this.Controls.Add(resumeBtn);
            this.Controls.Add(restartBtn);
            this.Controls.Add(exitBtn);

            resumeBtn.Click += (sender, e) => CloseMenu(form);
            restartBtn.Click += (sender, e) => RestartLevel(form);
            exitBtn.Click += (sender, e) => ExitLevel(form);
        }

        public void OpenMenu(GameForm form)
        {
            GameForm.GamePaused = true;
            form.GameTimer.Enabled = false;
            form.Program_StpWtch.Stop();
            form.MusicPlayer.Pause();
            this.Show();
            this.Enabled = true;
        }
        public void CloseMenu(GameForm form)
        {
            GameForm.GamePaused = false;
            form.GameTimer.Enabled = true;
            form.Program_StpWtch.Start();
            if (form.Program_StpWtch.ElapsedMilliseconds >= 3000)
                form.MusicPlayer.Play();
            this.Hide();
            this.Enabled = false;
        }

        public void RestartLevel(GameForm form)
        {
            if (form.ScoreMenu.Enabled)
            {
                form.ScoreMenu.Enabled = false;
                form.ScoreMenu.Hide();
                
            }
            form.Focus();
            form.StartLevel(new SongLevel(form.LevelData.LevelName, form.LevelData.LevelDifficulty));
            CloseMenu(form);
            
        }
        public void ExitLevel(GameForm form)
        {
            form.MainMenu.Location = form.Location;
            form.MainMenu.Show();
            form.Hide();
            this.Enabled = false;
            this.Hide();
        }
    }
}