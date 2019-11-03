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
        private const string notesTableName = "MyNotes"; // Ensure to have the same name in the Cloudformation template as well
        private const string metadataTableName = "MyNotesMetadata"; // Ensure to have the same name in the Cloudformation template as well

        private Table notesTable = null;
        private Table metadataTable = null;

        /// <summary>
        /// Private constructor - Singleton pattern
        /// </summary>
        private DynamoDBHelper()
        {
            
        }

        /// <summary>
        /// Returns the Single instance post doing some initializations
        /// </summary>
        /// <returns></returns>
        public static DynamoDBHelper Instance()
        {
            if (instance == null)
            {
                instance = new DynamoDBHelper();
            }

            LoadTableHandles();

            return instance;
        }

        /// <summary>
        /// Loads the DynamoDB table handles
        /// </summary>
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

        /// <summary>
        /// Inserts the notes into the appropriate DynamoDB table
        /// </summary>
        /// <param name="inputNotes"></param>
        /// <returns></returns>
        public bool InsertNotes(NotesStructure inputNotes)
        {
            try
            {
                var notes = new Document();

                foreach (var field in inputNotes.Notes)
                {
                    notes[field.Key] = field.Value; // Populate the notes Document from the input dictionary
                }

                PutItem(notes);

                return true;
            }
            catch (Exception ex)
            {
                Logger.AddLog(ex.ToString());
            }
            return false;
        }

        private async Task PutItem(Document notes)
        {
            try
            {
                var rsp = await notesTable.PutItemAsync(notes);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Fetch all the notes of a specific user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<NotesStructure> FetchNotes(string userId)
        {
            List<NotesStructure> notes = new List<NotesStructure>();

            var request = new QueryRequest
            {
                TableName = notesTableName,
                KeyConditions = new Dictionary<string, Condition>
                {
                    { Constants.USER_ID, new Condition()
                        {
                            ComparisonOperator = ComparisonOperator.EQ,
                            AttributeValueList = new List<AttributeValue>
                                {
                                new AttributeValue { S = userId }   // S means attribute is of type string
                                }
                        }
                    }
                },
            };
            var response = client.QueryAsync(request).Result;
            foreach (var item in response.Items)
            {
                NotesStructure notesStructure = new NotesStructure();
                foreach (var field in item)
                {
                    notesStructure.Notes[field.Key] = field.Value.S; // S implies a string type
                }

                notes.Add(notesStructure);
            }

            return notes;
        }

        /// <summary>
        /// Delete a specific notes item from DynamoDB
        /// </summary>
        /// <param name="notesToDelete"></param>
        /// <returns></returns>
        public bool DeleteItem(NotesStructure notesToDelete)
        {

            return false;
        }

        /// <summary>
        /// Update a specific notes item in DynamoDB
        /// </summary>
        /// <param name="notesToUpdate"></param>
        /// <returns></returns>
        public bool UpdateItem(NotesStructure notesToUpdate)
        {

            return false;
        }

        /// <summary>
        /// Helper method to print the contents of a Document object
        /// </summary>
        /// <param name="doc"></param>
        private static void PrintDocument(Document doc)
        {
            if (doc == null)
            {
                return;
            }

            foreach (var attribute in doc.GetAttributeNames())
            {
                string stringValue = null;
                var value = doc[attribute];
                if (value is Primitive)
                    stringValue = value.AsPrimitive().Value.ToString();
                else if (value is PrimitiveList)
                    stringValue = string.Join(",", (from primitive
                                    in value.AsPrimitiveList().Entries
                                                    select primitive.Value).ToArray());
                Logger.AddLog(string.Format("{0} - {1}", attribute, stringValue));
            }
        }

        /// <summary>
        /// Fetch the specified metadata field from the metadata DynamoDB table
        /// </summary>
        /// <param name="searchkey"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Insert or update the specified metadata field into the metadata DynamoDB table
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
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
