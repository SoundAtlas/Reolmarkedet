using Reolmarkedet.Core.Models;
using Reolmarkedet.WPF.Commands;
using System.Collections.ObjectModel;

namespace Reolmarkedet.WPF.ViewModels
{
    public class ShelfViewModel : ViewModelBase
    {
        public ObservableCollection<Shelf> Shelves { get; } = new();
        public ObservableCollection<ShelfType> ShelfTypes { get; } = new();

        private Shelf? _selectedShelf;
        private ShelfType? _selectedShelfType;

        public Shelf? SelectedShelf
        {
            get => _selectedShelf;
            set
            {
                if (_selectedShelf != value)
                {
                    _selectedShelf = value;
                    OnPropertyChanged();
                    SelectedShelfType = _selectedShelf?.ShelfType;
                }
            }
        }

        public ShelfType? SelectedShelfType
        {
            get => _selectedShelfType;
            set
            {
                if (_selectedShelfType != value)
                {
                    _selectedShelfType = value;
                    OnPropertyChanged();
                }

                UpdateShelfCommand.RaiseCanExecuteChanged();
                CancelUpdateShelfCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand UpdateShelfCommand { get; }
        public RelayCommand CancelUpdateShelfCommand { get; }

        public ShelfViewModel()
        {
            ShelfType sixShelves = new()
            {
                ShelfTypeId = 1,
                Name = "Six Shelves"
            };

            ShelfType threeShelvesWithClothesRail = new()
            {
                ShelfTypeId = 2,
                Name = "Three Shelves with Clothes Rail"
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

            UpdateShelfCommand = new RelayCommand(UpdateShelf, CanUpdateShelf);
            CancelUpdateShelfCommand = new RelayCommand(CancelUpdateShelf, CanCancelUpdateShelf);
        }

        private bool CanUpdateShelf(object? parameter)
        {
            return SelectedShelf is not null
                && SelectedShelfType is not null
                && SelectedShelfType != SelectedShelf.ShelfType;
        }

        private void UpdateShelf(object? parameter)
        {
            if (SelectedShelf is null || SelectedShelfType is null)
            {
                return;
            }

            SelectedShelf.ShelfType = SelectedShelfType;

            SelectedShelf = null;
        }

        private bool CanCancelUpdateShelf(object? parameter)
        {
            return SelectedShelf is not null;
        }

        private void CancelUpdateShelf(object? parameter)
        {
            SelectedShelf = null;
        }
    }
}
