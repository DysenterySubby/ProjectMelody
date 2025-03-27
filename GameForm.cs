using RhythmGame.codes.custom_button;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace RhythmGame
{
    public partial class GameForm : Form
    {
        //Static GameForm Properties
   
        public static long holdElpsd;
        public static bool GamePaused = false;

        public Timer GameTimer;

        public SongLevel LevelData;

        private List<NoteLine> HighwayList = new List<NoteLine>();
        public Dictionary<Keys?, GtrButton> ButtonDictionary = new Dictionary<Keys?, GtrButton>();

        private long shield_endTime;
        private bool shieldActivated = false;

        private bool songPlay = false;
        
        public int Score = 0;
        public int Streak = 0;

        private Label shieldTimerLbl;

        

        private Image MultiplierImage
        {
            get
            {
                if (Streak >= 10 && Streak < 20) { return new Bitmap(Image.FromFile($"{Program.ScoreBoardAssetsDirectory}\\combo_2x.png"), new Size(229, 189)); }
                else if (Streak >= 20 && Streak < 30) { return new Bitmap(Image.FromFile($"{Program.ScoreBoardAssetsDirectory}\\combo_4x.png"), new Size(229, 189)); }
                else if (Streak >= 30 && Streak < 40) { return new Bitmap(Image.FromFile($"{Program.ScoreBoardAssetsDirectory}\\combo_6x.png"), new Size(229, 189)); }
                else if (Streak >= 40 && Streak < 50) { return new Bitmap(Image.FromFile($"{Program.ScoreBoardAssetsDirectory}\\combo_8x.png"), new Size(229, 189)); ; }
                else if (Streak >= 50) { return new Bitmap(Image.FromFile($"{Program.ScoreBoardAssetsDirectory}\\combo_10x.png"), new Size(229, 189)); }
                else { return new Bitmap(Image.FromFile($"{Program.ScoreBoardAssetsDirectory}\\combo_1x.png"), new Size(229, 189)); }
            }
        }
        private int Multiplier
        {
            get
            {
                if (Streak >= 10 && Streak < 20) { return 2; }
                else if (Streak >= 20 && Streak < 30) { return 4; }
                else if (Streak >= 30 && Streak < 40) { return 6; }
                else if (Streak >= 40 && Streak < 50) { return 8; }
                else if (Streak >= 50) { return 10; }
                else { return 1; }
            }
        }


        private Stopwatch holdButton_stpWtch;
        public Stopwatch Program_StpWtch;

        public System.Windows.Media.MediaPlayer MusicPlayer;

        public Menu MainMenu;
        public ScoreMenu ScoreMenu;
        public PauseMenu GamePauseMenu;
        public GameForm(Menu mainMenu)
        {
            MainMenu = mainMenu;
            Program_StpWtch = new Stopwatch();
            holdButton_stpWtch = new Stopwatch();

            #region Initilizes Game Form Properties
            this.Text = "Project Melody";
            this.BackgroundImageLayout = ImageLayout.Stretch;
            
            this.Size = new Size(1000, 700);
            this.BackgroundImage = new Bitmap(Image.FromFile($"{Program.BackgroundAssetsDirectory}\\background_level_fretboard.png"), new Size(1000, 700));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            #endregion

            #region Initializes Game Form Menus
            GamePauseMenu = new PauseMenu(this);
            GamePauseMenu.Location = new Point(0, 0);
            this.Controls.Add(GamePauseMenu);
            ScoreMenu = new ScoreMenu(this);
            ScoreMenu.Location = new Point(0, 0);
            this.Controls.Add(ScoreMenu);
            #endregion

            #region Initializes User Interface Elements
            var imageLoc = Directory.GetFiles(Program.ButtonAssetsDirectory, "*.png").ToList();
            foreach (var loc in imageLoc)
            {
                GtrButton newBtn = new GtrButton(loc);
                ButtonDictionary.Add(GtrButton.GetKey(newBtn), newBtn);
                Controls.Add(newBtn);
            }

            shieldTimerLbl = new Label
            {
                AutoSize = true,
                Font = new Font("Algerian", 21, FontStyle.Bold),
                Location = new Point(Width / 2 - 50, 0),
                BackColor = Color.Transparent,
                ForeColor = Color.White
            };
            this.Controls.Add(shieldTimerLbl);

            #endregion

            #region Core Game Events
            GameTimer = new Timer();
            GameTimer.Interval = 30;
            GameTimer.Tick += new EventHandler(GameTimerEvent);

            this.KeyDown += new KeyEventHandler(ButtonDownEvent);
            this.KeyUp += new KeyEventHandler(ButtonUpEvent);
            
            this.Paint += new PaintEventHandler(Note_Paint);
            this.FormClosed += new FormClosedEventHandler(FormClosedEvent);
            #endregion

        }

        public  async void StartLevel(SongLevel songLevelData)
        {
            MainMenu.Hide();

            Score = 0;
            Streak = 0;
            if(ButtonDictionary.Count > 1)
            {
                Console.WriteLine("CLEARING BUTTOON DICTIONARY!");
                foreach (GtrButton gtrButton in ButtonDictionary.Values)
                    gtrButton.Dispose();
                ButtonDictionary.Clear();
            }

            var imageLoc = Directory.GetFiles(Program.ButtonAssetsDirectory, "*.png").ToList();
            foreach (var loc in imageLoc)
            {
                GtrButton newBtn = new GtrButton(loc);
                ButtonDictionary.Add(GtrButton.GetKey(newBtn), newBtn);
                Controls.Add(newBtn);
            }

            this.Location = MainMenu.Location;

            songPlay = false;

            LevelData = songLevelData;
            MusicPlayer = songLevelData.musicPlayer;

            Program_StpWtch.Reset();
            HighwayList.Clear();

            await Task.Run(() => LevelData.LoadLevel(ButtonDictionary));

            GameTimer.Start();
            Program_StpWtch.Start();
        }

        //Game Time Event, all of game logic comes from this event.s
        private void GameTimerEvent(object sender, EventArgs e)
        {
            //Variables used for calculations
            holdElpsd = holdButton_stpWtch.ElapsedMilliseconds;

            if (!songPlay && Program_StpWtch.ElapsedMilliseconds > 3000)
            {
                
                MusicPlayer.Play();
                songPlay = true;
            }
            else if (songPlay && Program_StpWtch.ElapsedMilliseconds >= MusicPlayer.NaturalDuration.TimeSpan.TotalMilliseconds + 5000)
            {

                MusicPlayer.Stop();
                MusicPlayer.Close();
                ScoreMenu.OpenScoreMenu(this);
            }


            //For Inserting Notes in the U.I.
            if (LevelData.SongList.Count> 0)
            {
                foreach (NoteLine noteLine in LevelData.SongList.ToArray())
                {
                    if (Program_StpWtch.ElapsedMilliseconds + 3000 >= noteLine.songPosition)
                    {
                        HighwayList.Add(noteLine);
                        LevelData.SongList.Remove(noteLine);
                    }
                }
            }
            
            if(HighwayList.Count > 0)
            {
                foreach (NoteLine noteLine in HighwayList.ToArray())
                {
                    float progress = (float)(noteLine.songPosition - Program_StpWtch.ElapsedMilliseconds) / LevelData.SongSpeed;
                    int noteY = (int)Math.Round(0 + ((0 - 550) * progress));
                    //---INPUT AND SCORING EVALUATION---
                    //Single Note Input Check
                    if (noteLine.isActive && !noteLine.isHoldType && noteLine.ButtonDown(shieldActivated))
                    {
                        if(noteLine.Type != Note.Standard)
                            EvaluateSpecialNote(noteLine.Type);
                        
                        Streak++;
                        Score += noteLine.Points * Multiplier;
                    }
                    //Hold Note Input Check
                    else if (noteLine.isActive && noteLine.isHoldType && noteLine.ButtonHold(shieldActivated))
                    {
                        //Computation for how much points the user will be given
                        double noteHoldElpsd = (double)(holdElpsd +40) / 100;
                        double noteHoldTime = Math.Ceiling((double)holdElpsd / 100);

                        //Increase Streak on the very first time collision is detected
                        if (noteLine.StreakEnabled)
                            Streak++;
                        //Increases Score by 1 for every 100th milliseconds
                        if (noteHoldElpsd > noteHoldTime)
                        {
                            Score += noteLine.Points * Multiplier ;
                        }
                    }
                    if (noteLine.isMiss)
                    {
                        Streak = 0;
                        if (noteLine.Type != Note.Standard && LevelData.SpecialNotesList.Count >= 1)
                        {
                            foreach (NoteLine specialNL in LevelData.SpecialNotesList.Peek())
                                specialNL.UpdateLine();
                            LevelData.SpecialNotesList.Dequeue();
                        }
                    }

                    noteLine.Animate(ref HighwayList, noteY, 550);
                }
            }
            //Resets the state of the buttons if the player tries to hold it down.
            foreach (GtrButton btn in ButtonDictionary.Values)
                if (btn.isDown && holdButton_stpWtch.ElapsedMilliseconds >= GtrButton.downHoldLimit)
                    btn.isDown = false;


            if (shieldActivated)
                UpdateShieldTimer();
            else
                shieldTimerLbl.Text = "";
            this.Invalidate();
        }

        
        private void Note_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            foreach (NoteLine noteLine in HighwayList)
            {
                foreach (Note note in noteLine.NoteList)
                {
                    if (noteLine.isHoldType)
                    {
                        INormalNote holdNote = (INormalNote)note;
                        e.Graphics.FillRectangle(new SolidBrush(holdNote.Color), holdNote.Line.Location.X, holdNote.Line.Location.Y, holdNote.Line.Width, holdNote.Line.Height);
                    }
                    if (note.Location.Y <= 500)
                        e.Graphics.DrawImage(note._noteImage, note.Location.X, note.Location.Y, 50, 50);
                }
            }
            if (GamePaused)
                foreach (GtrButton btn in ButtonDictionary.Values)
                    e.Graphics.DrawImage(btn.IdlePicture, btn.Location.X, btn.Location.Y, 80, 80);

            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawImage(MultiplierImage, new Point(0, 0));
            e.Graphics.DrawString(Score.ToString(), new Font("Algerian", 34, FontStyle.Bold), new SolidBrush(Color.White), 115, 100, sf);
            e.Dispose();
        }
        //Key Down Event
        private void ButtonDownEvent(object sender, KeyEventArgs e)
        {
            if (ButtonDictionary.ContainsKey(e.KeyData))
                GtrButton.KeyDownEvaluate(ButtonDictionary[e.KeyData]);
            holdButton_stpWtch.Start();
        }

        //Key Up Event
        private void ButtonUpEvent(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                this.Invalidate();
                if (!GamePaused)
                    GamePauseMenu.OpenMenu(this);
                else
                    GamePauseMenu.CloseMenu(this);
            }
            
            if (ButtonDictionary.ContainsKey(e.KeyData))
                GtrButton.KeyUpEvaluate(ButtonDictionary[e.KeyData]);
            holdButton_stpWtch.Reset();
        }

        private void UpdateShieldTimer()
        {
            if (Program_StpWtch.ElapsedMilliseconds >= shield_endTime)
            {
                shieldActivated = false;
                shield_endTime = 0;
            }
            shieldTimerLbl.Text = $"Shield Time Left: {(int)(shield_endTime - Program_StpWtch.ElapsedMilliseconds) / 1000}";
            
        }
        private void EvaluateSpecialNote(string noteType)
        {
            switch (noteType)
            {
                case Note.ComboShield:
                    if (!shieldActivated)
                    {
                        shieldActivated = true;
                        shield_endTime = Program_StpWtch.ElapsedMilliseconds + 8000;
                    }
                    break;
                case Note.InstantCombo:
                    Streak = 50;
                    break;
            }
        }

        private void FormClosedEvent(object sender, FormClosedEventArgs e)
        {
            MusicPlayer.Stop();
            MusicPlayer.Close();
            foreach (Control control in Controls)
                control.Dispose();
            GameTimer.Dispose();
            if (e.CloseReason == CloseReason.UserClosing)
                Environment.Exit(1);
        }

        


    }
}