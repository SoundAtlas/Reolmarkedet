using Reolmarkedet.Core.Models;
using Reolmarkedet.Core.Services;
using System.Collections.ObjectModel;

namespace Reolmarkedet.WPF.ViewModels
{
    public class RentalViewModel : ViewModelBase
    {
        public ObservableCollection<Tenant> Tenants { get; }
        public ObservableCollection<Shelf> Shelves { get; }
        public ObservableCollection<Rental> Rentals { get; }
        public ObservableCollection<Tenant> ActiveTenants { get; } = new();
        public ObservableCollection<Shelf> AvailableShelves { get; } = new();

        private readonly RentalService _rentalService = new();

        private Tenant? _selectedTenant;
        private Shelf? _selectedShelf;
        private DateTime? _startDate = DateTime.Today;
        private DateTime? _endDate;
        private string _monthlyRent = string.Empty;
        private string _rentalMessage = string.Empty;

        public Tenant? SelectedTenant
        {
            get => _selectedTenant;
            set
            {
                if (_selectedTenant != value)
                {
                    _selectedTenant = value;
                    OnPropertyChanged();
                }
            }
        }

        public Shelf? SelectedShelf
        {
            get => _selectedShelf;
            set
            {
                if (_selectedShelf != value)
                {
                    _selectedShelf = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged();
                    RefreshActiveTenants();
                    RefreshAvailableShelves();
                }
            }
        }

        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged();
                    RefreshActiveTenants();
                    RefreshAvailableShelves();
                }
            }
        }

        public string MonthlyRent
        {
            get => _monthlyRent;
            set
            {
                if (_monthlyRent != value)
                {
                    _monthlyRent = value;
                    OnPropertyChanged();
                }
            }
        }

        public string RentalMessage
        {
            get => _rentalMessage;
            private set
            {
                if (_rentalMessage != value)
                {
                    _rentalMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public RentalViewModel(
            ObservableCollection<Tenant> tenants,
            ObservableCollection<Shelf> shelves,
            ObservableCollection<Rental> rentals)
        {
            Tenants = tenants;
            Shelves = shelves;
            Rentals = rentals;
            Refresh();
        }
        public void Refresh()
        {
            RefreshActiveTenants();
            RefreshAvailableShelves();
        }
        private void RefreshActiveTenants()
        {
            ActiveTenants.Clear();

            foreach (var tenant in Tenants)
            {
                if (tenant.IsActive)
                {
                    ActiveTenants.Add(tenant);
                }
            }

            // If the currently selected tenant is no longer active, clear the selection
            if (SelectedTenant is not null &&
                !ActiveTenants.Contains(SelectedTenant))
            {
                SelectedTenant = null;
            }
        }

        private void RefreshAvailableShelves()
        {
            AvailableShelves.Clear();

            // If the start date is not set, we cannot determine available shelves, so we clear the selection and return
            if (StartDate is null)
            {
                SelectedShelf = null;
                return;
            }

            if (EndDate.HasValue &&
                EndDate.Value.Date < StartDate.Value.Date)
            {
                RentalMessage = "Slutdatoen kan ikke være før startdatoen.";
                return;
            }

            RentalMessage = string.Empty;

            foreach (var shelf in Shelves)
            {
                bool isAvailable = _rentalService.IsShelfAvailable(
                    shelf, StartDate.Value, EndDate, Rentals);

                if (isAvailable)
                {
                    AvailableShelves.Add(shelf);
                }
            }

            // If the currently selected shelf is no longer available, clear the selection
            if (SelectedShelf is not null &&
                !AvailableShelves.Contains(SelectedShelf))
            {
                SelectedShelf = null;
            }
        }
    }
}
