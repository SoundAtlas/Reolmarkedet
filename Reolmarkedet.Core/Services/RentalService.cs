using Reolmarkedet.Core.Models;


//RentalService
//- Create a rental
//- Check whether a shelf is available
//- Calculate correct rental price
//- Handle termination dates


namespace Reolmarkedet.Core.Services
{
    public class RentalService
    {
        public bool IsShelfAvailable(
            Shelf shelf,
            DateTime startDate,
            DateTime? endDate,
            IEnumerable<Rental> existingRentals)
        {
            // Compare calender dates only, ignoring time 
            DateTime requestedStart = startDate.Date;

            // No end date means the requested rental continues indefinitely
            // This value is only used for comparison 
            DateTime requestedEnd = endDate?.Date ?? DateTime.MaxValue.Date;

            // The end can equal the start, but cannot come before it
            if (requestedEnd < requestedStart)
            {
                throw new ArgumentException(
                    "End date cannot be before start date",
                    nameof(endDate));
            }

            foreach (Rental existingRental in existingRentals)
            {
                // Rentals for other shelves cannot block this shelf
                if (existingRental.Shelf.ShelfId != shelf.ShelfId)
                {
                    continue;
                }

                DateTime existingStart = existingRental.StartDate.Date;
                DateTime existingEnd =
                    existingRental.EndDate?.Date ?? DateTime.MaxValue.Date;

                // These are the only two cases where the requested rental does not overlap with an existing rental:
                bool endsBeforeExistingStart = requestedEnd < existingStart;
                bool startsAfterExistingEnd = requestedStart > existingEnd;

                // If neither is true, the periods share at least one day
                if (!endsBeforeExistingStart && !startsAfterExistingEnd)
                {
                    return false; // Shelf is not available
                }
            }

            // Every rental was checked and none blocked the requested period
            return true;
        }

        public Rental? GetCurrentRentalByShelfAndDate(
            Shelf shelf,
            DateTime date,
            IEnumerable<Rental> rentals)
        {
            DateTime day = date.Date;

            foreach (var rental in rentals)
            {
                bool sameShelf = rental.Shelf.ShelfId == shelf.ShelfId;
                bool hasStarted = rental.StartDate.Date <= day;
                bool hasNotEnded = rental.EndDate?.Date >= day ||
                    rental.EndDate is null;

                if (sameShelf && hasStarted && hasNotEnded)
                {
                    return rental;
                }
            }

            return null;
        }

        public ShelfStatus GetShelfStatus(
            Shelf shelf,
            DateTime date,
            IEnumerable<Rental> rentals)
        {
            var currentRental = GetCurrentRentalByShelfAndDate(shelf, date, rentals);

            if (currentRental is null)
            {
                return ShelfStatus.Available;
            }
            else if (currentRental.TerminationNoticeDate is not null &&
                     currentRental.TerminationNoticeDate.Value.Date <= date.Date)
            {
                return ShelfStatus.TerminationPending;
            }
            else
            {
                return ShelfStatus.Rented;
            }
        }
    }
}
