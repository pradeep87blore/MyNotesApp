using AWSHelpers;
using Common;
using System;

namespace ResourceInitializer
{
    class Program
    {
        static void Main(string[] args)
        {
            InitializeS3();

            InitializeLambda();
        }

        private static void InitializeLambda()
        {
           // throw new NotImplementedException();
        }

        private static void InitializeS3()
        {
            var bucketName = DynamoDBHelper.Instance().FetchMetadata(Constants.TOP_LEVEL_BUCKET_NAME_METADATA_KEY);
            if(string.IsNullOrEmpty(bucketName)) // The bucket doesnt exist yet, create it now
            {
                bool bucketCreated = false;
                do
                {
                    var hashValue = Math.Abs(DateTime.Now.ToString().GetHashCode());
                    var newBucketName = Constants.APPNAME + hashValue.ToString();
                    newBucketName = newBucketName.ToLower();
                    bucketCreated = S3Helper.Instance().CreateS3Bucket(newBucketName);

                    if(bucketCreated)
                    {
                        UpdateMetadataWithS3BucketName(newBucketName);
                    }
                }
                while (bucketCreated == false);
            }
        }

        private static void UpdateMetadataWithS3BucketName(string newBucketName)
        {
            DynamoDBHelper.Instance().InsertMetadata(Constants.TOP_LEVEL_BUCKET_NAME_METADATA_KEY, newBucketName);
        }

    }
}
