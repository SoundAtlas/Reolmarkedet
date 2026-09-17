namespace Reolmarkedet.Core.Models
{
    public class Rental
    {
        public int RentalId { get; set; }
        public Tenant Tenant { get; set; }
        public Shelf Shelf { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal MonthlyRent { get; set; }

        public Rental(Tenant tenant, Shelf shelf)
        {
            Tenant = tenant;
            Shelf = shelf;
        }
    }
}
