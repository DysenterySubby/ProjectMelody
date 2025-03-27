using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RhythmGame
{
    public partial class GtrButton : PictureBox
    {
        public const int downHoldLimit = 150;

        public Image IdlePicture;
        public Image ActivePicture;
        
        public bool isInvalidated = false;
        public bool isHolding = false;
        public bool isDown = false;

        public static Dictionary<string, Keys?> KeyBindDictionary = new Dictionary<string, Keys?>()
        {
            {"green", Keys.A},
            {"red", Keys.S},
            {"yellow", Keys.J},
            {"blue", Keys.K},
            {"orange", Keys.L}
        };

        private string _color;
        public string Color { get { return _color; } }
        public GtrButton(string imageLocation)
        {
            IdlePicture = Image.FromFile(imageLocation);
            ActivePicture = Image.FromFile($"{imageLocation.Remove(imageLocation.LastIndexOf('_'))}_pressed.gif");
            _color = imageLocation.Substring(imageLocation.LastIndexOf('\\') + 1); _color = _color.Remove(_color.IndexOf('_'));
            this.Size = new Size(80, 80);
            this.Image = IdlePicture;
            this.BackColor = System.Drawing.Color.Transparent;
            this.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        public static Keys? GetKey(GtrButton btn)
        {
            Keys? key = null;
            Point location = Point.Empty;
            switch (btn.Color)
            {
                case "green":
                    location = new Point(250, 500);
                    key = KeyBindDictionary["green"];
                    break;
                case "red":
                    location = new Point(350, 500);
                    key = KeyBindDictionary["red"];
                    break;
                case "yellow":
                    location = new Point(450, 500);
                    key = KeyBindDictionary["yellow"];
                    break;
                case "blue":
                    location = new Point(550, 500);
                    key = KeyBindDictionary["blue"];
                    break;
                case "orange":
                    location = new Point(650, 500);
                    key = KeyBindDictionary["orange"];
                    break;
            }

            btn.Location = location;
            return key;
        }

        public static Keys? GetKey(char noteColor)
        {
            Keys? key = null;
            switch (noteColor)
            {
                case 'g':
                    key = KeyBindDictionary["green"];
                    break;
                case 'r':
                    key = KeyBindDictionary["red"];
                    break;
                case 'y':
                    key = KeyBindDictionary["yellow"];
                    break;
                case 'b':
                    key = KeyBindDictionary["blue"];
                    break;
                case 'o':
                    key = KeyBindDictionary["orange"];
                    break;
            }

            return key;
        }

        public static void KeyDownEvaluate(GtrButton btn)
        {
            //Plays the button animation if pressed
            if (!btn.isHolding)
            {
                btn.Image = btn.ActivePicture;
            }

            //Invalidates key press if the player holds the key down
            if (GameForm.holdElpsd >= downHoldLimit)
                btn.isDown = false;
            else
                btn.isDown = true;

            //Invalidates the key hold press if the player tries to hold the key down just after a note has just been deactivated
            if (btn.isInvalidated)
                btn.isHolding = false;
            else
                btn.isHolding = true;
        }
        public static void KeyUpEvaluate(GtrButton btn)
        {
            btn.Image = btn.IdlePicture;
            btn.isDown = false;
            btn.isHolding = false;
            btn.isInvalidated = false;
        }
    }
}
