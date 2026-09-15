using Microsoft.Data.SqlClient;

namespace Reolmarkedet.Data.Database
{
    public class DatabaseConnectionTester
    {
        private readonly string _connectionString;

        public DatabaseConnectionTester(string connectionString)
        {
            _connectionString = connectionString;
        }


        public async Task TestConnectionAsync()
        {
            // Create the connection 
            await using SqlConnection connection = new(_connectionString);

            // Open the connection
            await connection.OpenAsync();

            // Defines a simple query - returns 1 if the connection is successful
            const string sql = "SELECT 1";

            // Creates a sql command 
            await using SqlCommand command = new(sql, connection);

            // Executes the command and retrieves the result
            object? result = await command.ExecuteScalarAsync();

            // Checks the result and throws an exception if it's not as expected
            if (result is not int value || value != 1)
            {
                throw new InvalidOperationException(
                    "The database connection test returned an unexpected result.");
            }
        }

    }
}
