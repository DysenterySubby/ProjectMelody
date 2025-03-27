using System;
using System.Drawing;
using System.Windows.Forms;

namespace RhythmGame
{
    internal class StandardNote : Note, INormalNote
    {
        
        protected HoldLine _line { get; set; }
        public HoldLine Line
        {
            get { return _line; }
            set
            {
                _line = value;
                _line.Location = new Point(_xPos + 20, 0);
                _line.BackColor = Color;
            }
        }

        public string _colorString { get; set; }
        public Color Color { get; set; }

        public StandardNote(GtrButton assignedButton)
        {
            _type = Note.Standard;
            _gtrBtn = assignedButton;
            _xPos = assignedButton.Location.X + 16;
            Color = Note.GetColor(assignedButton.Color[0]);
            _colorString = Note.GetColorString(assignedButton.Color[0]);

            _noteImage = Image.FromFile($"{Program.NoteAssetsDirectory}\\{_colorString}_note_standard.png");

            this.Location = new Point(_xPos, 0);
            //this.ControlRemoved += new ControlEventHandler(OnNoteDestroy);
        }
        public override void Destroy()
        {
            _noteImage.Dispose();
            this.Dispose();
        }
    }
}
