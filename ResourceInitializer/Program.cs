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

            //InitializeLambda();

            Console.WriteLine("Done,  please exit this program");
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

                    var thumbnailBucketName = newBucketName + "-thumbnails";

                    bucketCreated = S3Helper.Instance().CreateS3Bucket(thumbnailBucketName);

                    if (bucketCreated)
                    {
                        UpdateMetadataWithS3ThumbailBucketName(thumbnailBucketName);
                    }

                }
                while (bucketCreated == false);
            }
            else
            {
                Console.WriteLine("Required S3 buckets already exist, not creating new ones now");
            }
        }

        private static void UpdateMetadataWithS3BucketName(string newBucketName)
        {
            DynamoDBHelper.Instance().InsertMetadata(Constants.TOP_LEVEL_BUCKET_NAME_METADATA_KEY, newBucketName);
        }

        private static void UpdateMetadataWithS3ThumbailBucketName(string thumbnailBucketName)
        {
            DynamoDBHelper.Instance().InsertMetadata(Constants.TOP_LEVEL_THUMBNAIL_BUCKET_NAME_METADATA_KEY, thumbnailBucketName);
        }

    }
}
