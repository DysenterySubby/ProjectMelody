using System.Windows.Forms;
using System.IO;
using System;

namespace RhythmGame
{
    internal class Program
    {
        public static string LevelsDirectory { get { return $"{Directory.GetCurrentDirectory()}\\levels"; } }
        public static string MenuAssetsDirectory { get { return $"{Directory.GetCurrentDirectory()}\\assets\\menu"; } }
        public static string ScoreBoardAssetsDirectory { get { return $"{Directory.GetCurrentDirectory()}\\assets\\scoreboard"; } }
        public static string ButtonAssetsDirectory { get { return $"{Directory.GetCurrentDirectory()}\\assets\\button"; } }
        public static string NoteAssetsDirectory { get { return $"{Directory.GetCurrentDirectory()}\\assets\\note"; } }
        public static string BackgroundAssetsDirectory { get { return $"{Directory.GetCurrentDirectory()}\\assets\\backgrounds"; } }
        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(Directory.GetParent( Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString());
            Application.Run(new Menu());
        }
    }
}