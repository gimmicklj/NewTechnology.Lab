using System.Linq;
using WebApi.Data.DataContext;
using WebApi.Dtos;

namespace WebApi.Services.Inventory
{
    public class InventoryService:IInventoryService
    {
        private readonly BMSDbContext dataContext;

        public InventoryService(BMSDbContext dataContext)
        {
            this.dataContext = dataContext;
        }

        /// <summary>
        /// 修改库存
        /// </summary>
        /// <param name="purchaseBook"></param>
        public void UpdateInventory(PurchaseBook purchaseBook)
        {
            var item = dataContext.Books.Select(c => new Entity.Book
            {
                Id = c.Id,
                Name = c.Name,
                Author = c.Author,
                ISBN = c.ISBN,
                Publisher = c.Publisher,
                Inventory = c.Inventory
            }).FirstOrDefault(c => c.Id == purchaseBook.Id);
            if(item != null)
            {
                item.Inventory.Amount -= purchaseBook.Quantity;
            }
            dataContext.Books.Update(item);
        }
    }
}
