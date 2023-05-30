using System;
using System.Collections.Generic;
using System.Text;

namespace ClientApp
{
    public class JWT
    {
        public string access_token { get; set; }

        public int expires_in { get; set; }
    }
}
