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
            base.OnStartup(e);

            string? connectionString =
                ConfigurationManager.ConnectionStrings["ReolmarkedetDB"]?.ConnectionString;

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                MessageBox.Show("The database connection is not configured");
                Shutdown();
                return;
            }

            DatabaseConnectionTester tester = new(connectionString);

            await tester.TestConnectionAsync();

            MessageBox.Show("Database connection successful.");

            MainWindow mainWindow = new();
            mainWindow.Show();
        }
    }

}
