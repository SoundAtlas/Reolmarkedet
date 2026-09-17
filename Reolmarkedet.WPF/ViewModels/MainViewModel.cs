using Reolmarkedet.WPF.Views;

namespace Reolmarkedet.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {

        public TenantViewModel TenantManagement { get; }
        public MainViewModel()
        {
            TenantManagement = new TenantViewModel();
        }
    }
}
