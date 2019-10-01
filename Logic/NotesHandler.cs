using AWSHelpers;
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
            notesStructure.Notes[Constants.USER_ID] = notes.UserId;
            notesStructure.Notes[Constants.TIMESTAMP] = notes.TimeStamp;
            notesStructure.Notes[Constants.NOTES_TEXT] = notes.NotesText;
            notesStructure.Notes[Constants.NOTES_TITLE] = notes.NotesTitle;
            if (!string.IsNullOrEmpty(notes.FilePath))
            {
                string keyName = "";
                if (!UploadFileToS3(notes.UserId, notes.FilePath, out keyName))
                {
                    Logger.AddLog("Failed to upload file to S3");
                    return false;
                }
                notesStructure.Notes[Constants.FILE] = keyName;                
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

        private static bool UploadFileToS3(string userId, string filePath, out string keyName)
        {
            var rsp = S3Helper.Instance().UploadFileToS3(Utils.GetBucketName(), filePath,  userId);
            keyName = rsp.Result;
            return true;
        }

        public static List<NotesContent> FetchNotes(string userId)
        {
            List<NotesContent> fetchedNotes = new List<NotesContent>();
            var rsp = DynamoDBHelper.Instance().FetchNotes(userId);

            foreach(var notes in rsp)
            {
                NotesContent content = new NotesContent(null);
                content.UserId = notes.Notes[Constants.USER_ID];
                content.TimeStamp = notes.Notes[Constants.TIMESTAMP];
                content.NotesText = notes.Notes[Constants.NOTES_TEXT];                

                if (notes.Notes.ContainsKey(Constants.NOTES_TITLE))
                {
                    content.NotesTitle = notes.Notes[Constants.NOTES_TITLE];
                }

                if (notes.Notes.ContainsKey(Constants.FILE))
                {
                    content.FilePath = notes.Notes[Constants.FILE];
                }

                fetchedNotes.Add(content);
            }
            return fetchedNotes;
        }
    }
}
