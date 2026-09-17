using Reolmarkedet.WPF.ViewModels;
using System.Windows;

namespace Reolmarkedet.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainViewModel mainViewModel = new();

            MainWindow mainWindow = new(mainViewModel);
            mainWindow.Show();
        }
    }

}
