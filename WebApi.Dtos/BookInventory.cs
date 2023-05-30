namespace WebApi.Dtos
{
    public class BookInventory
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }

        public string ISBN { get; set; }

        public string Publisher { get; set; }

        public int Amount { get; set; }
    }
}
