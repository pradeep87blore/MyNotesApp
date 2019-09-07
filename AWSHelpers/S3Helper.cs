using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AWSHelpers
{
    public class S3Helper
    {
        AmazonS3Client client;

        static S3Helper instance;


        // Each User will have their own bucket

        private S3Helper()
        {
            client = new AmazonS3Client();
        }

        public static S3Helper Instance()
        {
            if (instance == null)
            {
                instance = new S3Helper();
            }


            return instance;
        }

        public async Task<bool> CreateS3BucketAsync(string bucketName)
        {
            try
            {
                if (!(await AmazonS3Util.DoesS3BucketExistV2Async(client, bucketName)))
                {
                    var putBucketRequest = new PutBucketRequest
                    {
                        BucketName = bucketName,
                        UseClientRegion = true
                    };

                    PutBucketResponse putBucketResponse = await client.PutBucketAsync(putBucketRequest);

                    return (putBucketResponse.HttpStatusCode == System.Net.HttpStatusCode.OK);
                }
            }
            catch (AmazonS3Exception e)
            {
                Logger.AddLog(string.Format("Error encountered on server. Message:'{0}' when writing an object", e.Message));
            }
            catch (Exception e)
            {
                Logger.AddLog(string.Format("Unknown encountered on server. Message:'{0}' when writing an object", e.Message));
            }

            return false;
        }

        public async Task<string> UploadFileToS3(string bucketName, string filePath)
        {
            var fileTransferUtility =
                    new TransferUtility(client);

            string keyName = Utils.GetKeyName(filePath);
            // Option 2. Specify object key name explicitly.
            fileTransferUtility.UploadAsync(filePath, bucketName, keyName);

            return keyName; // Shall be used to retrieve the item
        }

    }
}
