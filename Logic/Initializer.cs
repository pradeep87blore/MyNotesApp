using AWSHelpers;
using System;

namespace Logic
{
    public class Initializer
    {
        public static bool InitializeHelpers()
        {

            return InitializeDynamoDBHelper(); // Add more initializers here

        }
        private static bool InitializeDynamoDBHelper()
        {
            DynamoDBHelper.Instance();

            return false;
        }
    }
}
