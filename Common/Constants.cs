using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class Constants
    {
        public const int MAX_FILE_SIZE_KB = 200;

        public const string USER_ID = "UserID";
        public const string TIMESTAMP = "TimeStamp";
        public const string NOTES_TEXT = "NotesText";
        public const string NOTES_TITLE = "NotesTitle";
        public const string FILE = "File";
        public const string APPNAME = "PradeepBangaloreMyNotesApp";
        public const string KEY = "MetadataKey"; // The key used in the MyNotesMetadata table
        public const string VALUE = "Value"; // The value field name used in the MyNotesMetadata table

        public const string NEWEST_FIRST = "Newest First";
        public const string OLDEST_FIRST = "Oldest First";

        public const string MyNotesAppMetadataTable = "MyNotesMetadataTable";

        public const string TOP_LEVEL_BUCKET_NAME_METADATA_KEY = "S3TopLevelBucketName";
        public const string TOP_LEVEL_THUMBNAIL_BUCKET_NAME_METADATA_KEY = "S3TopLevelThumbnailBucketName";
    }
}
