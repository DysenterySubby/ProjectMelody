using RhythmGame.codes.custom_button;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RhythmGame.codes.sub_menu
{
    public partial class SettingMenu : UserControl
    {
        List<CustomButton> customButtons = new List<CustomButton>();
        CustomButton selectedButton;
        CustomButton returnButton;

        string selectedColor;
        public SettingMenu()
        {
            this.Size = new Size(1000, 700);
            this.BackColor = Color.FromArgb(200, 0, 0, 0);
            this.DoubleBuffered = true;
            int xOffset = 0;
            foreach (string color in GtrButton.KeyBindDictionary.Keys)
            {
                CustomButton newKey = new CustomButton($"{color}_keycap", new Size(80, 80), false)
                {
                    Text = GtrButton.KeyBindDictionary[color].ToString().ToUpper(),
                    Font = new Font("Sans", 20, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.White,
                    Location = new Point(270 + xOffset, 260)
                };
                newKey.Click += (object sender, EventArgs e) => KeySelect(newKey, color);
                SetKeyLocation(newKey);
                this.Controls.Add(newKey);

                customButtons.Add(newKey);
                xOffset += 100;
            }

            returnButton = new CustomButton("x", new Size(50, 30), false)
            {
                Location = new Point(920, 5)
            };
            returnButton.Click += (object sender, EventArgs e) => Close();
            this.Controls.Add(returnButton);

            this.Paint += new PaintEventHandler(SettingMenu_Paint);
            this.KeyDown += (object sendr, KeyEventArgs e) => SetKey(e);
        }
        private void SetKey(KeyEventArgs e)
        {
            if (selectedButton != null)
            {
                if (GtrButton.KeyBindDictionary.ContainsValue(e.KeyCode))
                {
                    foreach (CustomButton button in customButtons)
                        GtrButton.KeyBindDictionary[button.FileName.Substring(0, button.FileName.IndexOf('_'))] = e.KeyCode == GtrButton.KeyBindDictionary[button.FileName.Substring(0, button.FileName.IndexOf('_'))] ? GtrButton.KeyBindDictionary[selectedColor] : GtrButton.KeyBindDictionary[button.FileName.Substring(0, button.FileName.IndexOf('_'))];
                }
                GtrButton.KeyBindDictionary[selectedColor] = e.KeyCode;
                foreach (CustomButton button in customButtons)
                {
                    button.Text = GtrButton.KeyBindDictionary[button.FileName.Substring(0, button.FileName.IndexOf('_'))].ToString();
                }
            }
            selectedColor = null; selectedButton = null;
            foreach (string btnColor in GtrButton.KeyBindDictionary.Keys)
            {
                Console.WriteLine($"{btnColor.ToUpper()} Key Bind: Letter {GtrButton.KeyBindDictionary[btnColor].ToString()}");
            }
        }

        private void KeySelect(CustomButton slctBttn, string color)
        {
            foreach (CustomButton button in customButtons)
            {
                button.Text = button.FileName.Substring(0, button.FileName.IndexOf('_')) == color ? "" : GtrButton.KeyBindDictionary[button.FileName.Substring(0, button.FileName.IndexOf('_'))].ToString();
            }
            selectedColor = color;
            selectedButton = slctBttn;
        }

        private void SettingMenu_Paint(object sender, PaintEventArgs e)
        {
            
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Black)), 0, 0, 1000, 700);
            e.Graphics.DrawString("PRESS KEY TO SET", new Font("Sans", 30, FontStyle.Bold), new SolidBrush(Color.White), 300, 200);
        }
        public void Close()
        {
            this.Enabled = false;
            this.Visible = false;
        }

        public void Open()
        {
            
            this.Enabled = true;
            this.Visible = true;
            this.Focus();
        }

        private void SetKeyLocation(CustomButton keyButton)
        {
            switch (keyButton.FileName)
            {
                case "green_keycap":
                    keyButton.Location = new Point(260, 260);
                    break;
                case "red_keycap":
                    keyButton.Location = new Point(360, 260);
                    break;
                case "yellow_keycap":
                    keyButton.Location = new Point(460, 260);
                    break;
                case "blue_keycap":
                    keyButton.Location = new Point(560, 260);
                    break;
                case "orange_keycap":
                    keyButton.Location = new Point(660, 260);
                    break;
            }
        }
    }
}
