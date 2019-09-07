using AWSHelpers;
using Common;
using System;

namespace Logic
{
    public class Initializer
    {
        static string _userId;

        public async static void InitializeHelpers(string userId)
        {
            _userId = userId;

            S3Helper.Instance();

            await S3Helper.Instance().CreateS3BucketAsync(Utils.GetBucketName(userId));

            DynamoDBHelper.Instance();
        }
                
    }
}
