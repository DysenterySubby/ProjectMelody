using System;
using System.Drawing;
using System.Windows.Forms;

namespace RhythmGame
{
    internal class ComboShieldNote : Note
    {

        public ComboShieldNote(GtrButton assignedBtn)
        {
            _type = Note.ComboShield;
            _gtrBtn = assignedBtn;
            _xPos = assignedBtn.Location.X + 16;

            this.Location = new Point(_xPos, 0);
            _noteImage = Image.FromFile($"{Program.NoteAssetsDirectory}\\note_shield.png");
            Console.WriteLine("CREATED NEW SHIELD");

        }
        public override void Destroy()
        {
            _noteImage.Dispose();
            this.Dispose();
        }
    }
}