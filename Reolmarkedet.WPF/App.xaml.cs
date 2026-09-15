using Reolmarkedet.Data.Database;
using System.Configuration;
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

            // Waits while the application tests the database connection
            await tester.TestConnectionAsync();

            // Temporarily confirms that the connection test succeeded
            MessageBox.Show("Database connection successful.");

            // Opens the main window after the database test succeeds
            MainWindow mainWindow = new();
            mainWindow.Show();
        }
    }

}
