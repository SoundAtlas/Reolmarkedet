using Reolmarkedet.WPF.Commands;

namespace Reolmarkedet.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase? _currentViewModel;
        public ViewModelBase? CurrentViewModel
        {
            get { return _currentViewModel; }
            private set
            {
                if (_currentViewModel != value)
                {
                    _currentViewModel = value;
                    OnPropertyChanged();
                }
            }
        }

        public DashboardViewModel Dashboard { get; }
        public TenantViewModel TenantManagement { get; }

        public RelayCommand ShowDashboardCommand { get; }
        public RelayCommand ShowTenantsCommand { get; }

        public MainViewModel()
        {
            Dashboard = new DashboardViewModel();
            TenantManagement = new TenantViewModel();
            CurrentViewModel = Dashboard;

            ShowDashboardCommand =
                new RelayCommand(_ => CurrentViewModel = Dashboard);
            ShowTenantsCommand =
                new RelayCommand(_ => CurrentViewModel = TenantManagement);
        }
    }
}
