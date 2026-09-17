using Reolmarkedet.Core.Models;
using Reolmarkedet.WPF.Commands;
using System.Collections.ObjectModel;

namespace Reolmarkedet.WPF.ViewModels
{
    public class ShelfViewModel : ViewModelBase
    {
        public ObservableCollection<Shelf> Shelves { get; } = new();
        public ObservableCollection<ShelfType> ShelfTypes { get; } = new();

        private string _newShelfTypeName = string.Empty;
        public string NewShelfTypeName
        {
            get => _newShelfTypeName;
            set
            {
                if (_newShelfTypeName != value)
                {
                    _newShelfTypeName = value;
                    OnPropertyChanged();

                    AddShelfTypeCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public RelayCommand AddShelfTypeCommand { get; }

        public ShelfViewModel()
        {
            ShelfType sixShelves = new()
            {
                ShelfTypeId = 1,
                Name = "6 hylder"
            };

            ShelfType threeShelvesWithClothesRail = new()
            {
                ShelfTypeId = 2,
                Name = "3 hylder og bøjlestang"
            };

            ShelfTypes.Add(sixShelves);
            ShelfTypes.Add(threeShelvesWithClothesRail);

            Shelves.Add(new Shelf(sixShelves)
            {
                ShelfId = 1,
                ShelfNumber = 1
            });

            Shelves.Add(new Shelf(threeShelvesWithClothesRail)
            {
                ShelfId = 2,
                ShelfNumber = 2
            });

            AddShelfTypeCommand = new RelayCommand(AddShelfType, CanAddShelfType);
        }

        private int _nextShelfTypeId = 3;
        private void AddShelfType(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(NewShelfTypeName))
            {
                return;
            }

            ShelfType shelfType = new ShelfType()
            {
                ShelfTypeId = _nextShelfTypeId,
                Name = NewShelfTypeName.Trim()
            };

            ShelfTypes.Add(shelfType);
            _nextShelfTypeId++;
            NewShelfTypeName = string.Empty;
        }


        private bool CanAddShelfType(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(NewShelfTypeName);
        }


    }
}
