using System;
using System.Collections.Generic;
using System.Text;

namespace MainPage
{
    internal class TwitterUserInfo
    {
        public string Id { get; set; }
        public string Screen_name { get; set; } // The @ handle without the @
        public string profileImage { get; set; }
    }
}
