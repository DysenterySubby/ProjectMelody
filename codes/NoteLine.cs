using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace RhythmGame
{
    public class NoteLine
    {
        private List<Note> _noteList = new List<Note>();
        public List<Note> NoteList { get { return _noteList; } }

        public long songPosition;
        public bool isActive = false;
        private bool _isMiss = false;
        public bool isMiss { get { return _isMiss; } }

        private int _stateChangeCount = 0;

        public bool StreakEnabled
        {
            get
            {
                if (_stateChangeCount == 3)
                    return true;
                else
                    return
                        false;
            }
        }

        private int _holdDuration;
        public int HoldDuration
        {
            get { return _holdDuration; }
            private set
            {
                _holdDuration = value;
                _isHoldType = true;
            }
        }
        private bool _isHoldType = false;
        public bool isHoldType { get { return _isHoldType; } }

        public int Points { get { return _noteList.Count * _noteMultiplier; } }
        private int _noteMultiplier = 1;
        private string _type;
        public string Type
        {
            get { return _type; }
            private set
            {
                _type = value;
                switch (value)
                {
                    case Note.TwoX: _noteMultiplier = 2; break;
                    default: _noteMultiplier = 1; break;
                }
            }
        }

        //CONSTRUCTOR
        public NoteLine(string lineInfo, string noteType, Dictionary<System.Windows.Forms.Keys?, GtrButton> buttonDic)
        {
            Type = noteType;
            ParseLineInfo(lineInfo, noteType, buttonDic);
        }

        private void ParseLineInfo(string lineInfo, string noteType, Dictionary<System.Windows.Forms.Keys?, GtrButton> buttonDic)
        {
            var dataResult = lineInfo.Split(';');

            if (!long.TryParse(dataResult[0], out songPosition))
            {
                songPosition = Convert.ToInt64(dataResult[0].Split(':')[0]);
                HoldDuration = Convert.ToInt32(dataResult[0].Split(':')[1]);
            }

            var notesTemp =
                from note in dataResult[1]
                where Regex.IsMatch(Convert.ToString(note), @"[grybo]")
                select note;
            //creates note here
            foreach (var color in notesTemp)
            {
                Note newNote;
                switch (noteType)
                {
                    case Note.TwoX:
                        newNote = new TwoXNote(buttonDic[GtrButton.GetKey(color)]);
                        break;
                    case Note.ComboShield:
                        newNote = new ComboShieldNote(buttonDic[GtrButton.GetKey(color)]);
                        break;
                    case Note.InstantCombo:
                        newNote = new InstantComboNote(buttonDic[GtrButton.GetKey(color)]);
                        break;
                    default:
                        newNote = new StandardNote(buttonDic[GtrButton.GetKey(color)]);
                        break;
                }
                if (isHoldType && newNote is INormalNote normalNote)
                    normalNote.Line = new HoldLine(_holdDuration);
                _noteList.Add(newNote);
            }
        }

        //ANIMATES A ROW OF NOTE'S MOVEMENT FROM TOP TO BOTTOM
        int previousY;
        public void Animate(ref List<NoteLine> HighwayList, int newY, int endY)
        {
            if (!isActive && newY >= endY - 50 && _stateChangeCount == 0)
            {
                isActive = true;
                _stateChangeCount++;
            }
            foreach (Note note in _noteList)
            {
                note.Location = new Point(note.Location.X, newY);
                if (isHoldType && note is INormalNote)
                {
                    INormalNote holdNote = (INormalNote)note;
                    holdNote.Line.Location = new Point(holdNote.Line.Location.X, note.Location.Y - holdNote.Line.Size.Height);
                }
                //DISPOSES THE NOTE FROM THE FORM AFTER REACHING THE END Y LOCATION
                if (newY >= endY)
                {
                    if (isActive && _stateChangeCount == 1)
                        isActive = false;
                    if (!_isHoldType)
                        HighwayList.Remove(this);
                    note.Dispose();
                    note.Destroy();

                    //ANIMATES THE NOTE'S LINE OF THE HOLD NOTE BEFORE COMPLETELY DISPOSING IT
                    if (_isHoldType && note is INormalNote)
                    {
                        INormalNote holdNote = (INormalNote)note;
                        if (!isActive)
                            holdNote.Color = Color.Gray;
                        holdNote.Line.Size = new Size(10, holdNote.Line.Height - (newY - previousY));
                        holdNote.Line.Location = new Point(holdNote.Line.Location.X, endY - holdNote.Line.Size.Height);
                        if (holdNote.Line.Size.Height < 5)
                        {
                            holdNote.Line.Dispose();
                            HighwayList.Remove(this);
                        }
                    }
                }            }
            previousY = newY;
        }
        //Updates the type of note and it's  picture
        public void UpdateLine()
        {
            foreach (Note note in _noteList)
                if (note is IEditableNote iNote)
                    iNote.UpdateNote();
        }

        //HIT OR MISS NOTE CHECKER - FOR NORMAL TAP NOTES
        public bool ButtonDown(bool shieldActivated)
        {
            int trueCount = 0;
            foreach (Note note in _noteList)
            {
                if (note.GtrBtn.isDown)
                    trueCount++;
            }
            if (trueCount == _noteList.Count || shieldActivated)
            {
                isActive = false;
                return true;
            }
            _isMiss = true;
            return false;
        }

        //HIT OR MISS NOTE CHECKER - FOR HOLD NOTES
        public bool ButtonHold(bool shieldActivated)
        {
            int trueCount = 0;
            foreach (Note note in _noteList)
                if (note.GtrBtn.isHolding)
                    trueCount++;
            //Miss Input Validator
            if (_stateChangeCount == 1 && (trueCount != _noteList.Count || GameForm.holdElpsd >= _holdDuration) && !shieldActivated)
            {
                _isMiss = true;
                isActive = false;
            }
            //Hit Input Validator
            else if ((trueCount == _noteList.Count && GameForm.holdElpsd <= _holdDuration) || shieldActivated)
            {
                _stateChangeCount++;
                return true;
            }
            
            //Clears Button State
            foreach (Note note in _noteList)
            {
                note.GtrBtn.isInvalidated = true;
                note.GtrBtn.isHolding = false;
            }
            isActive = false;
            return false;
        }
    }
}

