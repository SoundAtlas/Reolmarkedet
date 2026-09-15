using Reolmarkedet.Core.Models;

namespace Reolmarkedet.Core.Interfaces
{
    public interface IItemRepository
    {
        // Checks if an item exists by barcode
        Task<Item?> GetByBarcodeAsync(string barcode);
    }
}
