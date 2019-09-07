using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class NotesContent
    {
        public string _notesText = "";

        public string _filePath = "";

        public string _userId = "";

        public string _timeStamp = "";

        override
            public string ToString()
        {
            string ret = "";

            if (string.IsNullOrEmpty(_filePath))
            {
                ret = string.Format("User ID: {0}, TimeStamp: {1}, Notes: {2}", _userId, _timeStamp, _notesText);
            }
            else
            {
                ret = string.Format("User ID: {0}, TimeStamp: {1}, Notes: {2}, File: {3}", _userId, _timeStamp, _notesText, _filePath);
            }

            return ret;
        }
    }
}
