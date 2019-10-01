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
        }
                
    }
}
