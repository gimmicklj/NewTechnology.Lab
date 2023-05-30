using System.Linq;

namespace WebApi.Services.Inventory
{
    public interface IInventoryService
    {
        /// <summary>
        /// 修改库存
        /// </summary>
        /// <param name="purchaseBook"></param>
        public void UpdateInventory(Dtos.PurchaseBook purchaseBook);
    }
}
