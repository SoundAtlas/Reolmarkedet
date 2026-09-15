using Reolmarkedet.Core.Interfaces;
using Reolmarkedet.Core.Models;

namespace Reolmarkedet.Data.Repositorires
{
    public class SqlSaleRepository : ISaleRepository
    {
        public Task<int> AddAsync(Sale sale)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsForItemAsync(int itemId)
        {
            throw new NotImplementedException();
        }
    }
}
