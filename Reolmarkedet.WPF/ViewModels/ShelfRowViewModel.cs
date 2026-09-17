using Reolmarkedet.Core.Models;

namespace Reolmarkedet.WPF.ViewModels
{
    public class ShelfRowViewModel
    {
        public Shelf Shelf { get; }
        public Rental? CurrentRental { get; }
        public ShelfStatus ShelfStatus { get; }
        public int ShelfNumber => Shelf.ShelfNumber;
        public ShelfType ShelfType => Shelf.ShelfType;


        public ShelfRowViewModel(Shelf shelf, Rental? currentRental, ShelfStatus shelfStatus)
        {
            Shelf = shelf;
            CurrentRental = currentRental;
            ShelfStatus = shelfStatus;
        }
    }
}
