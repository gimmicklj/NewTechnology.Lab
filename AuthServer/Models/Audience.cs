using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Audience
    {
        public string Secret { get; set; }
        public string Iss { get; set; }
        public string Aud { get; set; }
        public TimeSpan TokenExpiration {get; set;}
    }
}
