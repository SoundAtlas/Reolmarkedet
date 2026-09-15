using Microsoft.Data.SqlClient;
using Reolmarkedet.Data.Database;
using System.Configuration;
using System.Diagnostics;
using System.Windows;

namespace Reolmarkedet.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            // Runs the normal WPF startup process
            base.OnStartup(e);

            // Reads the database connection string from App.config
            string? connectionString =
                ConfigurationManager.ConnectionStrings["ReolmarkedetDB"]?.ConnectionString;

            // Stops the application if the connection string is missing
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                MessageBox.Show("The database connection is not configured.");
                Shutdown();
                return;
            }

            // Creates the connection tester using the configured connection string
            DatabaseConnectionTester tester = new(connectionString);

            try
            {
                // Waits while the application tests the database connection
                await tester.TestConnectionAsync();

                // Opens the main window after the database test succeeds
                MainWindow mainWindow = new();
                mainWindow.Show();
            }
            catch (SqlException ex)
            {
                // Logs the exception to the debug output
                Debug.WriteLine(ex);

                // Shows a message box to the user and shuts down the application
                MessageBox.Show(
                    "The application could not connect to the database. " +
                    "Check that SQL Server is running and try again.",
                    "Database connection error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Shutdown();
            }
        }
    }

}
