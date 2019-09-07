﻿using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AWSHelpers
{
    public class DynamoDBHelper
    {
        private static DynamoDBHelper instance;

        private AmazonDynamoDBClient client = new AmazonDynamoDBClient();
        private const string tableName = "MyNotesApp";

        private Table notesTable = null;

        private DynamoDBHelper()
        {
            try
            {
                CheckForAndCreateTable(); // Not adding an await since it can continue in the background                
            }
            catch(Exception ex)
            {
                Logger.AddLog(ex.ToString());
            }
        }

        private async Task CheckForAndCreateTable()
        {
            if (DoesTableAlreadyExist(tableName))
            {
                Logger.AddLog("Table " + tableName + " already exists");
            }
            else
            {
                var request = new CreateTableRequest
                {
                    TableName = tableName,
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition
                        {
                            AttributeName = "UserID",
                            AttributeType = "S"
                        },
                        new AttributeDefinition
                        {
                            AttributeName = "Timestamp",
                            AttributeType = "S"
                        }
                    },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "UserID",
                            KeyType = "HASH"
                        },
                        new KeySchemaElement
                        {
                            AttributeName = "Timestamp",
                            KeyType = "RANGE"
                        },
                    },
                    ProvisionedThroughput = new ProvisionedThroughput

                    {
                        ReadCapacityUnits = 2,
                        WriteCapacityUnits = 2
                    },
                };

                var response = client.CreateTableAsync(request);

                WaitUntilTableReady(tableName);
            }
        }

        private void WaitUntilTableReady(string tableName)
        {
            string status = null;
            // Let us wait until table is created. Call DescribeTable.
            do
            {
                System.Threading.Thread.Sleep(5000); // Wait 5 seconds.
                try
                {
                    var res = client.DescribeTableAsync(new DescribeTableRequest
                    {
                        TableName = tableName
                    });

                    Logger.AddLog(string.Format("Table name: {0}, status: {1}",
                              res.Result.Table.TableName,
                              res.Result.Table.TableStatus));
                    status = res.Result.Table.TableStatus;
                }
                catch (ResourceNotFoundException)
                {
                    // DescribeTable is eventually consistent. So you might
                    // get resource not found. So we handle the potential exception.
                }
            } while (status != "ACTIVE");
        }

        private bool DoesTableAlreadyExist(string tableName)
        {
            var currentTables = client.ListTablesAsync().Result.TableNames;

            return currentTables.Contains(tableName);
        }

        public static DynamoDBHelper Instance()
        {
            if (instance == null)
            {
                instance = new DynamoDBHelper();
            }

            // If the table is not loaded, load it up
            if(instance.notesTable == null)
            {
                instance.notesTable = Table.LoadTable(instance.client, tableName);
            }

            return instance;
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
            catch (Exception  ex)
            {
                Logger.AddLog(ex.ToString());
            }
            return false;
        }

    }
}