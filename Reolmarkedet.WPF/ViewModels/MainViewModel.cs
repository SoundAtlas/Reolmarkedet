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
        public ShelfViewModel ShelfManagement { get; }

        public RelayCommand ShowDashboardCommand { get; }
        public RelayCommand ShowTenantsCommand { get; }
        public RelayCommand ShowShelfManagementCommand { get; }

        public MainViewModel()
        {
            Dashboard = new DashboardViewModel();
            TenantManagement = new TenantViewModel();
            ShelfManagement = new ShelfViewModel();
            CurrentViewModel = Dashboard;

            ShowDashboardCommand =
                new RelayCommand(_ => CurrentViewModel = Dashboard);
            ShowTenantsCommand =
                new RelayCommand(_ => CurrentViewModel = TenantManagement);
            ShowShelfManagementCommand =
                new RelayCommand(_ => CurrentViewModel = ShelfManagement);
        }
    }
}
