using Reolmarkedet.Core.Models;

namespace Reolmarkedet.Core.Interfaces
{
    public interface IItemRepository
    {
        // Gets an item by barcode, or returns null when it does not exist.
        Task<Item?> GetByBarcodeAsync(string barcode);
    }
}
