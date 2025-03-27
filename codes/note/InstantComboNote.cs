using System;
using System.Drawing;
using System.Windows.Forms;

namespace RhythmGame
{
    internal class InstantComboNote : Note
    {

        public InstantComboNote(GtrButton assignedButton)
        {
            _type = Note.InstantCombo;
            _gtrBtn = assignedButton;
            _xPos = assignedButton.Location.X + 16;
            this.Location = new Point(_xPos, 0);
            _noteImage = Image.FromFile($"{Program.NoteAssetsDirectory}\\note_combo.png");
        }
        public override void Destroy()
        {
            _noteImage.Dispose();
            this.Dispose();
        }
    }
}
