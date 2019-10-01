using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSHelpers
{
    public class DynamoDBHelper
    {
        private static DynamoDBHelper instance;

        private AmazonDynamoDBClient client = new AmazonDynamoDBClient();
        private const string notesTableName = "MyNotes";
        private const string metadataTableName = "MyNotesMetadata";

        private Table notesTable = null;
        private Table metadataTable = null;

        private DynamoDBHelper()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Logger.AddLog(ex.ToString());
            }
        }

        public static DynamoDBHelper Instance()
        {
            if (instance == null)
            {
                instance = new DynamoDBHelper();
            }

            LoadTableHandles();

            return instance;
        }

        private static void LoadTableHandles()
        {
            // If the table is not loaded, load it up
            if (instance.notesTable == null)
            {
                instance.notesTable = Table.LoadTable(instance.client, notesTableName);
            }

            if (instance.metadataTable == null)
            {
                instance.metadataTable = Table.LoadTable(instance.client, metadataTableName);
            }

            if ((instance.metadataTable == null) || (instance.notesTable == null))
            {
                throw new ApplicationException("The required DB tables are not initialized. Run the appropriate cloud formation templates to get them initialized");
            }
        }

        public bool InsertNotes(NotesStructure inputNotes)
        {
            try
            {
                var notes = new Document();

                foreach (var field in inputNotes.Notes)
                {
                    notes[field.Key] = field.Value; // Populate the notes Document from the input dictionary
                }

                var rsp = notesTable.PutItemAsync(notes);

                return true;
            }
            catch (Exception ex)
            {
                Logger.AddLog(ex.ToString());
            }
            return false;
        }

        public List<NotesStructure> FetchNotes(string userId)
        {
            List<NotesStructure> notes = new List<NotesStructure>();

            QueryFilter filter = new QueryFilter(Constants.USER_ID, QueryOperator.Equal, userId);
            QueryOperationConfig config = new QueryOperationConfig()
            {
                Filter = filter,
                ConsistentRead = true
            };

            Search search = notesTable.Query(config);

            List<Document> documentSet = new List<Document>();
            do
            {
                documentSet = search.GetNextSetAsync().Result;
                Console.WriteLine("\nFindRepliesInLast15DaysWithConfig: printing ............");
                foreach (var document in documentSet)
                {
                    NotesStructure notesStructure = new NotesStructure();
                    foreach (var field in document)
                    {
                        notesStructure.Notes[field.Key] = field.Value;
                    }

                    notes.Add(notesStructure);
                }

            } while (!search.IsDone);

            return notes;
        }

        public bool DeleteItem(NotesStructure notesToDelete)
        {

            return false;
        }

        public bool UpdateItem(NotesStructure notesToUpdate)
        {

            return false;
        }

        private static void PrintDocument(Document updatedDocument)
        {
            foreach (var attribute in updatedDocument.GetAttributeNames())
            {
                string stringValue = null;
                var value = updatedDocument[attribute];
                if (value is Primitive)
                    stringValue = value.AsPrimitive().Value.ToString();
                else if (value is PrimitiveList)
                    stringValue = string.Join(",", (from primitive
                                    in value.AsPrimitiveList().Entries
                                                    select primitive.Value).ToArray());
                Logger.AddLog(string.Format("{0} - {1}", attribute, stringValue));
            }
        }

        public string FetchMetadata(string searchkey)
        {
            QueryFilter filter = new QueryFilter();
            filter.AddCondition(Constants.KEY, QueryOperator.Equal, searchkey);

            QueryOperationConfig config = new QueryOperationConfig();
            config.Select = SelectValues.AllAttributes;
            config.Filter = filter;

            Search searchRes = metadataTable.Query(config);
            List<Document> documentSet = new List<Document>();
            do
            {
                try
                {
                    // We can assume there is only one entry per key. 
                    // Hence, we are fetching the first item as required
                    documentSet = searchRes.GetNextSetAsync().Result;
                    Console.WriteLine("\nFindRepliesInLast15DaysWithConfig: printing ............");
                    return documentSet[0][Constants.VALUE];
                }
                catch(Exception ex)
                {
                    Logger.AddLog(ex.ToString());
                }
            } while (!searchRes.IsDone);

            return String.Empty;
        }

        public bool InsertMetadata(string key, string value)
        {
            try
            {
                // This creates a new entry if the same key didnt exist.
                // If the key existed, the value will be updated.
                var notes = new Document();

                notes[Constants.KEY] = key;
                notes[Constants.VALUE] = value;

                var rsp = metadataTable.PutItemAsync(notes);

                return true;
            }
            catch (Exception ex)
            {
                Logger.AddLog(ex.ToString());
            }
            return false;
        }

    }
}
