using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Dtos
{
    public class PurchaseBook
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }

        public string ISBN { get; set; }

        public string Publisher { get; set; }

        public int Quantity { get; set; }
    }
}
