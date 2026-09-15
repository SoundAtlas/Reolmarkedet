namespace Reolmarkedet.Core.Models
{
    public class Item
    {
        public int ItemId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public int RentalId { get; set; }
    }
}
