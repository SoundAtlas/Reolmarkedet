namespace Reolmarkedet.Core.Models
{
    public class Shelf
    {
        public int ShelfId { get; set; }
        public int ShelfNumber { get; set; }
        public ShelfType ShelfType { get; set; }

        public Shelf(ShelfType shelfType)
        {
            ShelfType = shelfType;
        }
    }
}
