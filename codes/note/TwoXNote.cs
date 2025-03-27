using System.Drawing;
using System.Windows.Forms;

namespace RhythmGame
{
    internal class TwoXNote : Note, INormalNote, IEditableNote
    {
        public string _colorString { get; set; }
        public Color Color { get; set; }

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
        

        public TwoXNote(GtrButton assignedBtn)
        {
            _type = Note.TwoX;
            _gtrBtn = assignedBtn;
            _xPos = assignedBtn.Location.X + 16;
            Color = Note.GetColor(assignedBtn.Color[0]);
            _colorString = Note.GetColorString(assignedBtn.Color[0]);

            _noteImage = Image.FromFile($"{Program.NoteAssetsDirectory}\\{_colorString}_note_twox.png");
            this.Location = new Point(_xPos, 0);
        }
        public override void Destroy()
        {
            _noteImage.Dispose();
            this.Dispose();
        }
        public void UpdateNote()
        {
            _noteImage = Image.FromFile($"{Program.NoteAssetsDirectory}\\{_colorString}_note_standard.png");
            this.Invalidate();
        }

    }
}
