using Reolmarkedet.Core.Models;
using System.Collections.ObjectModel;

namespace Reolmarkedet.WPF.ViewModels
{
    public class ShelfViewModel : ViewModelBase
    {
        public ObservableCollection<Shelf> Shelves { get; } = new();
        public ObservableCollection<ShelfType> ShelfTypes { get; } = new();

        public ShelfViewModel()
        {
            ShelfType sixShelves = new()
            {
                ShelfTypeId = 1,
                Name = "Six Shelves"
            };

            ShelfTypes.Add(sixShelves);

            new Shelf(sixShelves)
            {
                ShelfId = 1,
                ShelfNumber = 1
            };
        }
    }
}
