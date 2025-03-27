using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media;


namespace RhythmGame.codes.custom_button
{
    public  class SongLevel
    {
        public Queue<List<NoteLine>> SpecialNotesList = new Queue<List<NoteLine>>();
        public List<NoteLine> SongList = new List<NoteLine>();

        private string _levelDirectory;
        private string _levelDifficultyPath;
        private string _levelMusicPath;

        public string LevelName;
        public string LevelDifficulty;

        private int _songSpeed;
        public int SongSpeed { get { return _songSpeed; } }

        public string LevelDirectory { get { return _levelDirectory; } }
        public string DifficultyFile { get { return _levelDifficultyPath; } }
        public string MusicFilePath { get { return _levelMusicPath; } }


        public MediaPlayer musicPlayer;
        public SongLevel(string levelName, string levelDifficulty)
        {
            LevelName = levelName;
            LevelDifficulty = levelDifficulty;

            switch (levelDifficulty.ToLower())
            {
                case "novice":
                    _songSpeed = 1000;
                    break;
                case "amateur":
                    _songSpeed = 900;
                    break;
                case "expert":
                    _songSpeed = 800;
                    break;
            }
            _levelDirectory = $"{Program.LevelsDirectory}\\{levelName}";
            _levelDifficultyPath = $"{_levelDirectory}\\{levelDifficulty}.txt";
            _levelMusicPath = $"{_levelDirectory}\\{levelName}.wav";

            musicPlayer = new MediaPlayer();
            musicPlayer.MediaEnded += new EventHandler(BackgroundMusic_Ended);
            musicPlayer.Open(new Uri(new Uri(_levelMusicPath).AbsoluteUri));
            
        }
        private void BackgroundMusic_Ended(object    sender, EventArgs e)
        {
            musicPlayer.Pause();
        }


        public void LoadLevel(Dictionary<Keys?, GtrButton> buttonDic)
        {
            string noteType = Note.Standard;

            foreach (var line in File.ReadLines(DifficultyFile))
            {
                if (line.Contains("start"))
                {
                    noteType = line.Remove(line.IndexOf("|")).ToLower();
                    if (noteType != Note.Standard)
                        SpecialNotesList.Enqueue(new List<NoteLine>());
                    continue;
                }
                NoteLine noteLine = new NoteLine(line, noteType, buttonDic);
                if (noteType != Note.Standard)
                    SpecialNotesList.ElementAt(SpecialNotesList.Count - 1).Add(noteLine);
                SongList.Add(noteLine);
            }
        }

    }
}
