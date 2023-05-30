namespace WebApi.Entity
{
    public class Inventory
    {
        public int Id { get; set; }

        public int Amount { get; set; }

        public int Bookid { get; set; }

        public Book Book { get; set; }
    }
}
