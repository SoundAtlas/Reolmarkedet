using Reolmarkedet.Core.Models;
using Reolmarkedet.Core.Services;
using Reolmarkedet.WPF.Commands;
using System.Collections.ObjectModel;

namespace Reolmarkedet.WPF.ViewModels
{
    public class RentalViewModel : ViewModelBase
    {
        public ObservableCollection<Tenant> Tenants { get; }
        public ObservableCollection<Rental> Rentals { get; }
        public ObservableCollection<Tenant> ActiveTenants { get; } = new();
        public ObservableCollection<Shelf> Shelves { get; }
        public ObservableCollection<Shelf> AvailableShelves { get; } = new();
        public ObservableCollection<Shelf> SelectedShelves { get; } = new();

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

                    AddShelfToSelectionCommand.RaiseCanExecuteChanged();
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


        public RelayCommand AddShelfToSelectionCommand { get; }
        public RelayCommand RemoveShelfFromSelectionCommand { get; }

        public RentalViewModel(
            ObservableCollection<Tenant> tenants,
            ObservableCollection<Shelf> shelves,
            ObservableCollection<Rental> rentals)
        {
            Tenants = tenants;
            Shelves = shelves;
            Rentals = rentals;
            AddShelfToSelectionCommand =
                new RelayCommand(AddShelfToSelection, CanAddShelfToSelection);
            RemoveShelfFromSelectionCommand =
                new RelayCommand(RemoveShelfFromSelection, CanRemoveShelfFromSelection);
            Refresh();
        }

        private bool CanRemoveShelfFromSelection(object? parameter)
        {
            return parameter is Shelf shelf && SelectedShelves.Contains(shelf);
        }

        private void RemoveShelfFromSelection(object? parameter)
        {
            if (parameter is not Shelf shelf || !SelectedShelves.Contains(shelf))
            {
                return;
            }

            SelectedShelves.Remove(shelf);
            RefreshAvailableShelves();
        }

        private bool CanAddShelfToSelection(object? parameter)
        {
            return SelectedShelf is not null
                && !SelectedShelves.Contains(SelectedShelf)
                && AvailableShelves.Contains(SelectedShelf);
        }

        private void AddShelfToSelection(object? parameter)
        {
            if (SelectedShelf is null ||
                !CanAddShelfToSelection(parameter))
            {
                return;
            }

            Shelf shelf = SelectedShelf;
            SelectedShelves.Add(shelf);
            RefreshAvailableShelves();
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
                RentalMessage = "Vælg en startdato for at se tilgængelige reoler.";
                return;
            }

            if (EndDate.HasValue &&
                EndDate.Value.Date < StartDate.Value.Date)
            {
                SelectedShelf = null;
                RentalMessage = "Slutdatoen kan ikke være før startdatoen.";
                return;
            }

            RentalMessage = string.Empty;

            foreach (var shelf in Shelves)
            {
                bool isAvailable = _rentalService.IsShelfAvailable(
                    shelf, StartDate.Value, EndDate, Rentals);

                // If the shelf is available and not already selected, add it to the available shelves
                if (isAvailable && !SelectedShelves.Contains(shelf))
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
