using Reolmarkedet.Core.Models;

namespace Reolmarkedet.Core.Interfaces
{
    public interface ISaleRepository
    {
        // Checks if an item has a sale 
        Task<bool> ExistsForItemAsync(int itemId);

        // Adds a sale 
        Task<int> AddAsync(Sale sale);
    }
}
