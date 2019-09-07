using System;
using System.Collections.Generic;

namespace Common
{
    /// <summary>
    /// This class is what is passed to the DynamoDBHelper.
    /// The contents shall be saved into the table
    /// </summary>
    public class NotesStructure
    {
        public NotesStructure()
        {
            Notes = new Dictionary<string, string>();
            UserId = "";
        }
        public string UserId { get; set; } // The user who is saving the notes
        public Dictionary<string, string> Notes { get; set; } // Notes text and image resource
    }
}
