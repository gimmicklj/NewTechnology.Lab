using System;
using System.Collections.Generic;
using System.Text;

namespace AuthServer.Models
{
    public class Audience
    {
        public string Secret { get; set; }
        public string Iss { get; set; }
        public string Aud { get; set; }
    }
}
