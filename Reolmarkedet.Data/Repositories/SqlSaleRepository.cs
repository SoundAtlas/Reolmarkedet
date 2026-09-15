using Microsoft.Data.SqlClient;
using Reolmarkedet.Core.Interfaces;
using Reolmarkedet.Core.Models;
using System.Data;

namespace Reolmarkedet.Data.Repositories
{
    public class SqlSaleRepository : ISaleRepository
    {
        private readonly string _connectionString;

        public SqlSaleRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private const string AddSaleQuery = @"
            INSERT INTO dbo.SALE (SaleDate, SalePrice, ItemID)
            Output INSERTED.SaleID
            VALUES (@SaleDate, @SalePrice, @ItemID);";
        private const string ExistsForItemQuery = @"
            SELECT COUNT(*) 
            FROM dbo.SALE 
            WHERE ItemID = @ItemID;";


        public async Task<int> AddAsync(Sale sale)
        {
            await using SqlConnection connection = new(_connectionString);
            await connection.OpenAsync();

            await using SqlCommand command = new(AddSaleQuery, connection);

            // Converts the domain DateOnly to a parameter value;
            // SqlDbType.Date ensures that SQL Server stores only the date.
            command.Parameters.Add("@SaleDate", SqlDbType.Date).Value =
                sale.SaleDate.ToDateTime(TimeOnly.MinValue);

            SqlParameter salePriceParameter =
                command.Parameters.Add("@SalePrice", SqlDbType.Decimal);

            // Set the precision and scale for the decimal parameter
            salePriceParameter.Precision = 10;
            salePriceParameter.Scale = 2;
            salePriceParameter.Value = sale.SalePrice;

            command.Parameters.Add("@ItemID", SqlDbType.Int).Value = sale.ItemId;

            object? result = await command.ExecuteScalarAsync() ?? throw new InvalidOperationException(
                    "The sale was inserted without a returned SaleID.");

            return Convert.ToInt32(result);
        }


        public async Task<bool> ExistsForItemAsync(int itemId)
        {
            await using SqlConnection connection = new(_connectionString);
            await connection.OpenAsync();

            await using SqlCommand command = new(ExistsForItemQuery, connection);

            command.Parameters.Add("@ItemID", SqlDbType.Int).Value = itemId;

            object? result = await command.ExecuteScalarAsync();

            int count = Convert.ToInt32(result);

            return count > 0;

        }
    }
}
