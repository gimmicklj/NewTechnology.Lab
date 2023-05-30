using System.ComponentModel.DataAnnotations;

namespace WebApi.Entity
{
    public class Book
    {
        public int Id { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(64)]
        public string Author { get; set; }

        [MaxLength(32)]
        public string ISBN { get; set; }

        [MaxLength(64)]
        public string Publisher { get; set; }

        public Inventory Inventory { get; set; }
    }
}
