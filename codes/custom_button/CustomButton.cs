using System;
using System.Drawing;

namespace RhythmGame.codes.custom_button
{
    internal class CustomButton : System.Windows.Forms.Label
    {
        Rectangle imageRec;

        public bool IsSelected;
        public bool IsDisabled;

        private string _fileName;
        private bool _isRadioButton;
        public bool IsRadioButton
        {
            get { return _isRadioButton; }
            set
            {
                if (value == true)
                {
                    IsDisabled = false;
                    IsSelected = false;
                    _isRadioButton = true;
                }
                else
                    _isRadioButton = value;
            }
        }
        public string FileName { get { return _fileName; } }
        public CustomButton(string fileName, Size imageSize, bool isRB)
        {
            _fileName = fileName;
            IsRadioButton = isRB;
            Size = imageSize;
            imageRec.Size = imageSize;
            Image = new Bitmap(Image.FromFile($"{Program.MenuAssetsDirectory}\\{_fileName}_button.png"), imageRec.Size);
            ImageAlign = ContentAlignment.MiddleCenter;
            BackColor = Color.Transparent;

            this.MouseDown += MouseDownEvent;
            this.Click += MouseUpEvent;
            this.MouseEnter += MouseHoverEvent;
            this.MouseLeave += MouseHoverExitEvent;
        }

        private void MouseDownEvent(object sender, EventArgs e)
        {
            Image = new Bitmap(Image.FromFile($"{Program.MenuAssetsDirectory}\\{_fileName}_button_pressed.png"), imageRec.Size);
        }
        private void MouseUpEvent(object sender, EventArgs e)
        {
            if (IsRadioButton)
            {
                if (!IsSelected)
                {
                    Image = new Bitmap(Image.FromFile($"{Program.MenuAssetsDirectory}\\{_fileName}_button_hover.png"), imageRec.Size);
                    IsSelected = true;
                }
                else
                {
                    Image = new Bitmap(Image.FromFile($"{Program.MenuAssetsDirectory}\\{_fileName}_button.png"), imageRec.Size);
                    IsSelected = false;
                }
            }
            else
                Image = new Bitmap(Image.FromFile($"{Program.MenuAssetsDirectory}\\{_fileName}_button.png"), imageRec.Size);
        }

        private void MouseHoverEvent(object sender, EventArgs e)
        {
            Image = new Bitmap(Image.FromFile($"{Program.MenuAssetsDirectory}\\{_fileName}_button_hover.png"), imageRec.Size);
        }
        private void MouseHoverExitEvent(object sender, EventArgs e)
        {
            if (IsRadioButton)
            {
                if (IsSelected)
                    Image = new Bitmap(Image.FromFile($"{ Program.MenuAssetsDirectory }\\{ _fileName}_button_hover.png"), imageRec.Size);
                else if (IsDisabled)
                    Image = new Bitmap(Image.FromFile($"{Program.MenuAssetsDirectory}\\{_fileName}_button_pressed.png"), imageRec.Size);
                else
                    Image = new Bitmap(Image.FromFile($"{Program.MenuAssetsDirectory}\\{_fileName}_button.png"), imageRec.Size);
            }
            else
                Image = new Bitmap(Image.FromFile($"{Program.MenuAssetsDirectory}\\{_fileName}_button.png"), imageRec.Size);

        }
    }
}
