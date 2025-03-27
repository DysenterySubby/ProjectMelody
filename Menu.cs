using RhythmGame.codes.custom_button;
using RhythmGame.codes.sub_menu;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RhythmGame
{
    
    internal class SongFlowLayoutPanel : FlowLayoutPanel
    {
        public SongFlowLayoutPanel() : base()
        {
            this.DoubleBuffered = true;
            this.BackColor = Color.FromArgb(230, 0, 0, 0);
        }
        protected override void OnScroll(ScrollEventArgs se)
        {
            this.Invalidate();
            base.OnScroll(se);
        }
    }
    public partial class Menu : Form
    {
        private Image backgroundImage;
        private LevelSelectMenu levelSelect;
        private SettingMenu settingMenu;



        CustomButton startButton = new CustomButton("play", new Size(200, 50), false);
        CustomButton exitButton = new CustomButton("quit", new Size(200, 50), false);
        CustomButton settingButton = new CustomButton("setting", new Size(80, 80), false);
        public Menu()
        {
            #region Initialize Menu U.I
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            this.ClientSize = new Size(1000, 700);
            this.Text = "Project Melody";
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Size = new Size(1000, 700);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            #endregion

            GameForm newGameForm = new GameForm(this);
            newGameForm.StartPosition = FormStartPosition.Manual;
            newGameForm.Location = this.Location;

            levelSelect = new LevelSelectMenu(this, newGameForm);
            settingMenu = new SettingMenu();

            startButton.Location = new Point(390, 400);
            exitButton.Location = new Point(390, 460);
            settingButton.Location = new Point(890, 570);

            startButton.Click += (object sender, EventArgs e) => levelSelect.Open();
            exitButton.Click += (object sender, EventArgs e) => Environment.Exit(1);
            settingButton.Click += (object sender, EventArgs e) => settingMenu.Open();

            this.Controls.Add(settingMenu);
            this.Controls.Add(levelSelect);

            this.Controls.Add(startButton);
            this.Controls.Add(exitButton);
            this.Controls.Add(settingButton);

            backgroundImage = Image.FromFile($"{Program.BackgroundAssetsDirectory}\\background_menu.gif");

            this.Paint += new PaintEventHandler(Menu_Paint);
            this.FormClosed += new FormClosedEventHandler(ApplicationExit_Event);

            ImageAnimator.Animate(backgroundImage, AnimateBackgroundImage);
        }

        private void ApplicationExit_Event(object sender, FormClosedEventArgs e)
        {
            foreach (Control control in Controls)
                control.Dispose();
        }

        private void Menu_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(backgroundImage, 0, 0, 1000, 700);
            e.Graphics.DrawImage(new Bitmap($"{Program.MenuAssetsDirectory}\\logo.png"), 350, 150, 300, 144);
            ImageAnimator.UpdateFrames(backgroundImage);
        }

        private void AnimateBackgroundImage(object sender, EventArgs e)
        {
            this.Invalidate();
        }
    }
}
