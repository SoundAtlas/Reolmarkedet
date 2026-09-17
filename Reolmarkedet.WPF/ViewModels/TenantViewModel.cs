using Reolmarkedet.Core.Models;
using Reolmarkedet.Core.Services;
using Reolmarkedet.WPF.Commands;
using System.Collections.ObjectModel;

namespace Reolmarkedet.WPF.ViewModels
{
    public class TenantViewModel : ViewModelBase
    {
        // Observable collection to hold the list of tenants
        public ObservableCollection<Tenant> Tenants { get; } = new();
        public ObservableCollection<Tenant> VisibleTenants { get; } = new();
        public ObservableCollection<Rental> Rentals { get; }

        private readonly RentalService _rentalService = new();

        private string _searchText = string.Empty;
        private string _name = string.Empty;
        private string _phoneNumber = string.Empty;
        private string _email = string.Empty;
        private Tenant? _selectedTenant;
        private bool _showInactiveTenants;
        private string _validationMessage = string.Empty;


        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    ApplySearch();
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (_phoneNumber != value)
                {
                    _phoneNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged();
                }
            }
        }

        public Tenant? SelectedTenant
        {
            get => _selectedTenant;
            set
            {
                if (_selectedTenant != value)
                {
                    _selectedTenant = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FormTitle)); // Notify that FormTitle has changed
                    ValidationMessage = string.Empty; // Clear validation message when a tenant is selected

                    if (_selectedTenant is not null)
                    {
                        Name = _selectedTenant.Name;
                        PhoneNumber = _selectedTenant.PhoneNumber ?? string.Empty;
                        Email = _selectedTenant.Email ?? string.Empty;
                    }
                    else // Clear the form fields when no tenant is selected
                    {
                        ClearFormFields();
                    }

                    AddTenantCommand.RaiseCanExecuteChanged();
                    UpdateTenantCommand.RaiseCanExecuteChanged();
                    CancelUpdateTenantCommand.RaiseCanExecuteChanged();
                    DeleteTenantCommand.RaiseCanExecuteChanged();
                    DeactivateTenantCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool ShowInactiveTenants
        {
            get => _showInactiveTenants;
            set
            {
                if (_showInactiveTenants != value)
                {
                    _showInactiveTenants = value;
                    OnPropertyChanged();
                    SelectedTenant = null; // Clear the selected tenant when toggling the filter
                    ApplySearch();
                }
            }
        }

        public string ValidationMessage
        {
            get => _validationMessage;
            set
            {
                if (_validationMessage != value)
                {
                    _validationMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public string FormTitle => SelectedTenant is null ? "Opret reollejer" : $"Rediger: {SelectedTenant.Name}";

        public RelayCommand AddTenantCommand { get; }
        public RelayCommand UpdateTenantCommand { get; }
        public RelayCommand CancelUpdateTenantCommand { get; }
        public RelayCommand DeleteTenantCommand { get; }
        public RelayCommand DeactivateTenantCommand { get; }

        public TenantViewModel(ObservableCollection<Rental> rentals)
        {
            Rentals = rentals;
            AddTenantCommand = new RelayCommand(AddTenant, CanAddTenant);
            UpdateTenantCommand = new RelayCommand(UpdateTenant, CanUpdateTenant);
            CancelUpdateTenantCommand = new RelayCommand(CancelUpdateTenant, CanCancelUpdateTenant);
            DeleteTenantCommand = new RelayCommand(DeleteTenant, CanDeleteTenant);
            DeactivateTenantCommand = new RelayCommand(DeactivateTenant, CanDeactivateTenant);
        }

        private bool CanDeactivateTenant(object? parameter)
        {
            return SelectedTenant is not null && SelectedTenant.IsActive;
        }

        private void DeactivateTenant(object? parameter)
        {
            if (SelectedTenant is null)
            {
                return;
            }

            Tenant tenant = SelectedTenant;

            if (!tenant.IsActive)
            {
                return;
            }

            bool hasCurrentOrFutureRentals =
                _rentalService.HasCurrentOrFutureRentalsForTenant(
                    tenant, DateTime.Now, Rentals);

            if (hasCurrentOrFutureRentals)
            {
                ValidationMessage =
                    "Reollejeren har nuværende eller kommende lejemål og kan ikke deaktiveres.";
                return;
            }

            tenant.IsActive = false;
            SelectedTenant = null;
            ValidationMessage = string.Empty;
            ApplySearch();

        }

        private int _nextTenantId = 1;

        private bool CanAddTenant(object? parameter)
        {
            return SelectedTenant is null;
        }
        private void AddTenant(object? obj)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                ValidationMessage = "Navn må ikke være tomt.";
                return;
            }

            ValidationMessage = string.Empty;

            Tenant tenant = new Tenant()
            {
                TenantId = _nextTenantId,
                Name = Name,
                Email = Email,
                PhoneNumber = PhoneNumber,
            };

            Tenants.Add(tenant);
            ApplySearch();
            _nextTenantId++;

            SelectedTenant = null;
            ClearFormFields();
        }

        private bool CanUpdateTenant(object? parameter)
        {
            return SelectedTenant is not null;
        }


        private void UpdateTenant(object? parameter)
        {
            Tenant? tenant = SelectedTenant;
            if (tenant is null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                ValidationMessage =
                    "Navn må ikke være tomt. Angiv et navn, eller annuller redigeringen.";
                return;
            }

            ValidationMessage = string.Empty;

            tenant.Name = Name;
            tenant.PhoneNumber = string.IsNullOrWhiteSpace(PhoneNumber) ? null : PhoneNumber;
            tenant.Email = string.IsNullOrWhiteSpace(Email) ? null : Email;

            ApplySearch();
            SelectedTenant = null;
            ClearFormFields();
        }

        private bool CanCancelUpdateTenant(object? parameter)
        {
            return SelectedTenant is not null;
        }

        private void CancelUpdateTenant(object? parameter)
        {
            SelectedTenant = null;
            ClearFormFields();
        }

        private bool CanDeleteTenant(object? parameter)
        {
            return SelectedTenant is not null;
        }

        private void DeleteTenant(object? parameter)
        {
            Tenant? tenant = SelectedTenant;
            if (tenant is null)
            {
                return;
            }

            if (_rentalService.HasRentalsForTenant(tenant, Rentals))
            {
                ValidationMessage = "Reollejeren har tilknyttede lejemål og kan ikke slettes.";
                return;
            }

            Tenants.Remove(tenant);
            ApplySearch();
            SelectedTenant = null;
            ClearFormFields();
        }

        private void ClearFormFields()
        {
            Name = string.Empty;
            PhoneNumber = string.Empty;
            Email = string.Empty;
            ValidationMessage = string.Empty;
        }

        private void ApplySearch()
        {
            VisibleTenants.Clear();
            foreach (var tenant in Tenants)
            {
                // Check if the tenant matches the active/inactive filter
                bool matchesActivity =
                    (!ShowInactiveTenants && tenant.IsActive) ||
                    (ShowInactiveTenants && !tenant.IsActive);

                if (matchesActivity &&
                    (string.IsNullOrWhiteSpace(SearchText) ||
                    tenant.Name.Contains(SearchText.Trim(),
                        StringComparison.OrdinalIgnoreCase)))
                {
                    VisibleTenants.Add(tenant);
                }
            }
        }
    }
}
