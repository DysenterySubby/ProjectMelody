using System;
using System.Drawing;
using System.Windows.Forms;

namespace RhythmGame
{
    interface INormalNote
    {
        HoldLine Line { get; set; }
        string _colorString { get; set; }
        Color Color { get; set; }
    }
    interface IEditableNote
    {
        void UpdateNote();
    }

    public abstract class Note : Control
    {
        public const string Standard = "standard";
        public const string TwoX = "twox";
        public const string ComboShield = "comboshield";
        public const string InstantCombo = "instantcombo";


        public Image _noteImage;
        protected string _type;
        public string Type { get { return _type; } }

        public int _xPos;
        protected GtrButton _gtrBtn {  get; set; }
        public GtrButton GtrBtn { get { return _gtrBtn; } }
        public Note()
        {
            this.Size = new Size(50, 50);
        }

        public static Color GetColor(char noteColor)
        {
            Color color;
            switch (noteColor)
            {
                case 'g':
                    color = Color.Green;
                    break;
                case 'r':
                    color = Color.Red;
                    break;
                case 'y':
                    color = Color.Yellow;
                    break;
                case 'b':
                    color = Color.Blue;
                    break;
                case 'o':
                    color = Color.Orange;
                    break;
                default:
                    color = Color.Gray;
                    break;
            }
            return color;
        }

        public static string GetColorString(char noteColor)
        {
            string color;
            switch (noteColor)
            {
                case 'g':
                    color = "green";
                    break;
                case 'r':
                    color = "red";
                    break;
                case 'y':
                    color = "yellow";
                    break;
                case 'b':
                    color = "blue";
                    break;
                case 'o':
                    color = "orange";
                    break;
                default:
                    color = "gray";
                    break;
            }
            return color;
        }
        public abstract void Destroy();
    }

    internal class HoldLine : PictureBox
    {
        public HoldLine(int hldDrtn)
        {
            this.Size = new Size(10, Convert.ToInt32(70 * (hldDrtn / 100)));
        }
    }
}
