using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class NotesContent
    {
        private string _notesText = "";

        private string _filePath = "";

        private string _userId = "";

        private string _timeStamp = "";

        private string _notesTitle = "";

        public NotesContent(NotesContent selectedItem) // Do a deep copy (Copy Constructor)
        {
            if (selectedItem != null)
            {
                this.NotesText = new string(selectedItem.NotesText);
                this.FilePath = new string(selectedItem.FilePath);
                this.UserId = new string(selectedItem.UserId);
                this.TimeStamp = new string(selectedItem.TimeStamp);
                this.NotesText = new string(selectedItem.NotesText);
            }
        }

        public string NotesText { get => _notesText; set => _notesText = value; }
        public string FilePath { get => _filePath; set => _filePath = value; }
        public string UserId { get => _userId; set => _userId = value; }
        public string TimeStamp { get => _timeStamp; set => _timeStamp = value; }
        public string NotesTitle { get => _notesTitle; set => _notesTitle = value; }

        override
            public string ToString()
        {
            string ret = "";

            if (string.IsNullOrEmpty(FilePath))
            {
                ret = string.Format("User ID: {0}, TimeStamp: {1}, Notes: {2}", UserId, TimeStamp, NotesText);
            }
            else
            {
                string fileName = Utils.GetFileNameFromKeyName(FilePath);
                ret = string.Format("User ID: {0}, TimeStamp: {1}, Notes: {2}, File: {3}", UserId, TimeStamp, NotesText, fileName);
            }

            return ret;
        }
    }
}
