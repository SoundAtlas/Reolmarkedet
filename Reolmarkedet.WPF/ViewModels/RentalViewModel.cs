using Reolmarkedet.Core.Models;
using System.Collections.ObjectModel;

namespace Reolmarkedet.WPF.ViewModels
{
    public class RentalViewModel : ViewModelBase
    {
        public ObservableCollection<Tenant> Tenants { get; }
        public ObservableCollection<Shelf> Shelves { get; }
        public ObservableCollection<Rental> Rentals { get; }

        public RentalViewModel(
            ObservableCollection<Tenant> tenants,
            ObservableCollection<Shelf> shelves,
            ObservableCollection<Rental> rentals)
        {
            Tenants = tenants;
            Shelves = shelves;
            Rentals = rentals;
        }
    }
}
