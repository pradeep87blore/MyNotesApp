﻿using AWSHelpers;
using Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logic
{
    public class NotesHandler
    {
        public async static Task<bool> AddNotes(NotesContent notes)
        {
            NotesStructure notesStructure = new NotesStructure();
            notesStructure.Notes[Constants.USER_ID] = notes._userId;
            notesStructure.Notes[Constants.TIMESTAMP] = DateTime.Now.ToString();
            notesStructure.Notes[Constants.NOTES_TEXT] = notes._notesText;
            if (!string.IsNullOrEmpty(notes._filePath))
            {
                if (!UploadFileToS3(notes._filePath))
                {
                    Logger.AddLog("Failed to upload file to S3");
                    return false;
                }
                notesStructure.Notes[Constants.FILE] = notes._filePath;                
            }

            var insertResult = DynamoDBHelper.Instance().InsertNotes(notesStructure);

            if (insertResult)
            {
                Logger.AddLog("Successfully added notes");
            }
            else
            {
                Logger.AddLog("Failed to added notes");
            }
            return insertResult;
        }

        private static bool UploadFileToS3(string filePath)
        {
            // TODO: Add logic here
            return true;
        }

        public static List<NotesContent> FetchNotes(string userId)
        {
            List<NotesContent> fetchedNotes = new List<NotesContent>();
            var rsp = DynamoDBHelper.Instance().FetchNotes(userId);

            foreach(var notes in rsp)
            {
                NotesContent content = new NotesContent();
                content._userId = notes.Notes[Constants.USER_ID];
                content._timeStamp = notes.Notes[Constants.TIMESTAMP];
                content._notesText = notes.Notes[Constants.NOTES_TEXT];

                if (notes.Notes.ContainsKey(Constants.FILE))
                {
                    content._filePath = notes.Notes[Constants.FILE];
                }

                fetchedNotes.Add(content);
            }
            return fetchedNotes;
        }
    }
}
