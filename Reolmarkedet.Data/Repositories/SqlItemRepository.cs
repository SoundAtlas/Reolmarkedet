using Reolmarkedet.Core.Interfaces;
using Reolmarkedet.Core.Models;

namespace Reolmarkedet.Data.Repositorires
{
    public class SqlItemRepository : IItemRepository
    {
        public Task<Item?> GetByBarcodeAsync(string barcode)
        {
            throw new NotImplementedException();
        }
    }
}
