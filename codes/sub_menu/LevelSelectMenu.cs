using RhythmGame.codes.custom_button;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RhythmGame
{
    public partial class LevelSelectMenu : UserControl
    {
        private SongFlowLayoutPanel flowLayoutPanel;
        private PictureBox albumPictureBox;

        List<string> songs = new List<string>();
        Panel contentPanel;
        CustomButton difficultyDropDownButton;
        List<CustomButton> difficultyButtons = new List<CustomButton>();
        List<CustomButton> levelButtons = new List<CustomButton>();
        CustomButton startButton;
        CustomButton returnButton;

        Dictionary<string, int> leaderBoard = new Dictionary<string, int>();

        private string SelectedSong = null;
        private string SelectedDifficulty = null;
        public LevelSelectMenu(Menu mainMenu, GameForm gameForm)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);

            this.Enabled = false;
            this.Visible = false;
            this.Size = new Size(1000, 700);
            this.BackColor = Color.Transparent;

            songs = Directory.GetDirectories(Program.LevelsDirectory).Select(Path.GetFileName).ToList();

            InitializeSongButtons();
            InitializeUserInterface();
            InitializeButtons(gameForm);

            this.Paint += new PaintEventHandler(LevelSelectMenu_Paint);
        }

        private void StartGame(GameForm GameForm)
        {
            GameForm.StartLevel(new SongLevel(SelectedSong, SelectedDifficulty));
            GameForm.ShowDialog();
        }
        private void UpdateState()
        {
            if (SelectedSong != null && SelectedDifficulty != null)
            {
                startButton.Enabled = true;
            }
        }


        #region Initializes Song Selector Buttons
        private void InitializeSongButtons()
        {
            flowLayoutPanel = new SongFlowLayoutPanel
            {
                WrapContents = false,
                Parent = this,
                BorderStyle = BorderStyle.FixedSingle,

                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                Location = new Point(-1, 0),
                Name = "flowLayoutPanel",
                TabIndex = 0,
                Size = new Size(310, 700)
            };

            foreach (var song in songs)
            {
                CustomButton newButton = new CustomButton("level", new Size(300, 100), true)
                {
                    Text = song.ToUpper(),
                    TextAlign = ContentAlignment.MiddleCenter,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    Font = new Font("Arial", 18, FontStyle.Bold)
                };
                newButton.Click += (sender, e) => LevelButtonClick(song);
                flowLayoutPanel.Controls.Add(newButton);
                levelButtons.Add(newButton);
            }
            Controls.Add(flowLayoutPanel);
        }

        private void LevelButtonClick(string songName)
        {
            string temp = songName.Substring(songName.LastIndexOf(" ") + 1);
            Image imagePath = new Bitmap($"{Program.LevelsDirectory}\\{songName}\\AlbumCover.png");
            albumPictureBox.Image = imagePath;
            albumPictureBox.Visible = true;
            foreach (CustomButton button in levelButtons)
            {
                button.Image = button.Text.Substring(button.Text.LastIndexOf(" ") + 1) == temp.ToUpper() ? new Bitmap(Image.FromFile($"{Program.MenuAssetsDirectory}\\{button.FileName}_button_hover.png"), new Size(300, 100)) : new Bitmap(Image.FromFile($"{Program.MenuAssetsDirectory}\\{button.FileName}_button_pressed.png"), new Size(300, 100));
                button.IsSelected = button.Text.Substring(button.Text.LastIndexOf(" ") + 1) == temp.ToUpper() ? true : false;
            }
            SelectedSong = songName;
            UpdateState();
        }
        #endregion

        #region Initialize Navigation Buttons
        private void InitializeButtons(GameForm gameForm)
        {
            #region Initializes U.I And Navigation Buttons
            startButton = new CustomButton("start", new Size(200, 50), false)
            {
                Location = new Point(715, 550),
                Enabled = false,
            };
            startButton.Click += (sender, e) => StartGame(gameForm);
            returnButton = new CustomButton("x", new Size(50, 30), false)
            { 
                Location = new Point(920, 5)
            };
            returnButton.Click += (object sender, EventArgs e) => Close();

            this.Controls.Add(returnButton);
            this.Controls.Add(startButton);
            #endregion
        }
        #endregion

        #region Initializes U.I Elements of Selected Level
        private void InitializeUserInterface()
        {
            albumPictureBox = new PictureBox
            {
                Image = Image.FromFile($"{Program.MenuAssetsDirectory}\\no_album.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(350, 350),
                Location = new Point(flowLayoutPanel.Width + 10, 10),

            };
            this.Controls.Add(albumPictureBox);

            contentPanel = new Panel
            {
                Location = new Point(715, 70 + 50),
                Size = new Size(200, 170),
                BackColor = Color.Transparent,
                BorderStyle = BorderStyle.None,
                Visible = false
            };

            difficultyDropDownButton = new CustomButton("difficulty_down", new Size(200, 50), true)
            {
                Location = new Point(715, 10 + 50)
            };
            difficultyDropDownButton.Click += ToggleDropdown;

            string[] difficultyArray = { "novice", "amateur", "expert" };
            int yOffset = 0;
            foreach (string difficulty in difficultyArray)
            {
                CustomButton newButton = new CustomButton($"{difficulty}", new Size(200, 50), true)
                {
                    Location = new Point(0, yOffset)
                };
                newButton.Click += (sender, e) => ToggleRadioButton(difficulty);
                newButton.Parent = contentPanel;
                yOffset += 50 + 10;
                difficultyButtons.Add(newButton);
            }
            Controls.Add(contentPanel);
            Controls.Add(difficultyDropDownButton);
        }

        private void ToggleDropdown(object sender, EventArgs e)
        {
            contentPanel.Visible = !contentPanel.Visible;
        }

        private void ToggleRadioButton(string selectedDifficulty)
        {
            foreach (CustomButton button in difficultyButtons)
            {
                button.Image = button.FileName == selectedDifficulty ? new Bitmap(Image.FromFile($"{Program.MenuAssetsDirectory}\\{button.FileName}_button_hover.png"), new Size(200, 50)) : new Bitmap(Image.FromFile($"{Program.MenuAssetsDirectory}\\{button.FileName}_button_pressed.png"), new Size(200, 50));
                button.IsSelected = button.FileName == selectedDifficulty ? true : false;
                button.IsDisabled = button.FileName == selectedDifficulty ? false : true;
            }

            SelectedDifficulty = selectedDifficulty;
            UpdateState();
        }
        #endregion

        public void Open()
        {
            this.Enabled = true;
            this.Visible = true;
        }
        public void Close()
        {
            this.Enabled = false;
            this.Visible = false;
        }

        private void LevelSelectMenu_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(200, Color.Black)), flowLayoutPanel.Width - 1, 0, Width - flowLayoutPanel.Width, 700);
            if (SelectedSong != null)
                e.Graphics.DrawString($"Song: {SelectedSong.Substring(SelectedSong.LastIndexOf(" ") + 1).ToUpper()}\n", new Font("Ariel", 26, FontStyle.Bold), new SolidBrush(Color.White), new Point(flowLayoutPanel.Width + 10, albumPictureBox.Location.Y + albumPictureBox.Height + 10));

            if (SelectedDifficulty != null && SelectedDifficulty != null && File.Exists($"{Program.LevelsDirectory}\\{SelectedSong}\\{SelectedDifficulty}Leaderboard.txt"))
            {

                leaderBoard.Clear();
                using (StreamReader read = new StreamReader($"{Program.LevelsDirectory}\\{SelectedSong}\\{SelectedDifficulty}Leaderboard.txt"))
                {
                    string line;
                    while ((line = read.ReadLine()) != null)
                    {
                        if (line != string.Empty)
                        {
                            int score = int.Parse(line.Split(':')[0]);
                            string name = line.Split(':')[1];
                            if (leaderBoard.ContainsKey(name) && score >= leaderBoard[name])
                                leaderBoard[name] = score;
                            else if (!leaderBoard.ContainsKey(name))
                                leaderBoard.Add(name, score);
                        }
                    }
                }
                var sortedDict = from entry in leaderBoard orderby entry.Value descending select entry;
                int i = 0;
                int yOffset = 0;
                foreach (var data in sortedDict)
                {
                    if (i < 5)
                    {
                        e.Graphics.DrawString($"{data.Key}: {data.Value}", new Font("Ariel", 18, FontStyle.Bold), new SolidBrush(Color.White), new Point(flowLayoutPanel.Width + 15, albumPictureBox.Location.Y + albumPictureBox.Height + 70 + yOffset));
                        yOffset += 30;
                        i++;
                    }
                }
            }
        }
    }
}
