using Microsoft.Data.SqlClient;
using Reolmarkedet.Core.Interfaces;
using Reolmarkedet.Core.Models;
using System.Data;


namespace Reolmarkedet.Data.Repositories
{
    public class SqlItemRepository : IItemRepository
    {
        // Stored configuration used to create a new SQL Server connection for each operation.
        private readonly string _connectionString;

        // Receives the connection string from the application's startup code.
        // The repository does not read App.config directly.
        public SqlItemRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Selects the item whose unique barcode matches the supplied SQL parameter.
        // @Barcode is a placeholder, its value is added to the command separately.
        const string GetByBarcodeQuery = @"
            SELECT ItemID, Description, Price, Barcode, RentalID
            FROM dbo.ITEM
            WHERE Barcode = @Barcode";

        // Retrieves one item by barcode.
        // Returns null when the database contains no matching item.
        public async Task<Item?> GetByBarcodeAsync(string barcode)
        {
            await using SqlConnection connection = new(_connectionString);
            await connection.OpenAsync();

            await using SqlCommand command = new(GetByBarcodeQuery, connection);

            // Gives the SQL @Barcode placeholder the value received by this C# method paramter
            // Sending it as a parameter prevents user input from becoming executable SQL.
            command.Parameters.Add("@Barcode", SqlDbType.NVarChar, 50).Value = barcode;

            // Executes the SELECT query and provides access to the returned rows and columns.
            await using SqlDataReader reader = await command.ExecuteReaderAsync();

            // Moves the reader to the first returned row.
            // It returns false when no item has the requested barcode.
            bool hasItem = await reader.ReadAsync();

            if (!hasItem)
            {
                // An unknown barcode is an expected result, so represent it with null.
                return null;
            }

            // Translates the database row into the Item model used by Core and the rest of the application.
            return new Item
            {
                ItemId = reader.GetInt32("ItemID"),
                Description = reader.GetString("Description"),
                Price = reader.GetDecimal("Price"),
                Barcode = reader.GetString("Barcode"),
                RentalId = reader.GetInt32("RentalID")
            };
        }
    }
}

