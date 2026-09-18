using Reolmarkedet.Core.Models;

namespace Reolmarkedet.WPF.ViewModels
{
    public class RentalRowViewModel : ViewModelBase
    {
        public Rental Rental { get; }
        public int RentalId => Rental.RentalId;
        public string TenantName => Rental.Tenant.Name;
        public int ShelfNumber => Rental.Shelf.ShelfNumber;
        public DateTime StartDate => Rental.StartDate;
        public DateTime? EndDate => Rental.EndDate;
        public decimal MonthlyRent => Rental.MonthlyRent;


        public RentalRowViewModel(Rental rental)
        {
            Rental = rental;
        }
    }
}
