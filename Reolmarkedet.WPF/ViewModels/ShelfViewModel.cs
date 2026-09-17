using Reolmarkedet.Core.Models;
using Reolmarkedet.Core.Services;
using Reolmarkedet.WPF.Commands;
using System.Collections.ObjectModel;

namespace Reolmarkedet.WPF.ViewModels
{
    public class ShelfViewModel : ViewModelBase
    {
        public ObservableCollection<Shelf> Shelves { get; } = new();
        public ObservableCollection<ShelfRowViewModel> VisibleShelves { get; } = new();
        public ObservableCollection<ShelfType> ShelfTypes { get; } = new();
        public ObservableCollection<Rental> Rentals { get; }

        private readonly RentalService _rentalService = new();
        private string _newShelfTypeName = string.Empty;
        private ShelfType? _newShelfType;
        private ShelfType? _shelfTypeToDelete;
        private string _shelfTypeMessage = string.Empty;
        private string _newShelfNumber = string.Empty;
        private string _shelfMessage = string.Empty;
        private ShelfRowViewModel? _selectedShelfRow;
        private ShelfStatusFilter _selectedStatusFilter = ShelfStatusFilter.All;

        public ShelfStatusFilter SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set
            {
                if (_selectedStatusFilter != value)
                {
                    _selectedStatusFilter = value;
                    OnPropertyChanged();

                    ApplyShelfFilter();
                }
            }
        }


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

        public ShelfType? ShelfTypeToDelete
        {
            get => _shelfTypeToDelete;
            set
            {
                if (_shelfTypeToDelete != value)
                {
                    _shelfTypeToDelete = value;
                    OnPropertyChanged();

                    DeleteShelfTypeCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string ShelfTypeMessage
        {
            get => _shelfTypeMessage;
            set
            {
                _shelfTypeMessage = value;
                OnPropertyChanged();
            }
        }

        public string NewShelfNumber
        {
            get => _newShelfNumber;
            set
            {
                if (_newShelfNumber != value)
                {
                    _newShelfNumber = value;
                    OnPropertyChanged();
                    ValidateShelfNumber();
                    AddShelfCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string ShelfMessage
        {
            get => _shelfMessage;
            set
            {
                _shelfMessage = value;
                OnPropertyChanged();
            }
        }

        public ShelfType? NewShelfType
        {
            get => _newShelfType;
            set
            {
                if (_newShelfType != value)
                {
                    _newShelfType = value;
                    OnPropertyChanged();

                    AddShelfCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ShelfRowViewModel? SelectedShelfRow
        {
            get => _selectedShelfRow;
            set
            {
                if (_selectedShelfRow != value)
                {
                    _selectedShelfRow = value;
                    OnPropertyChanged();

                    DeleteShelfCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public RelayCommand AddShelfTypeCommand { get; }
        public RelayCommand DeleteShelfTypeCommand { get; }
        public RelayCommand AddShelfCommand { get; }
        public RelayCommand DeleteShelfCommand { get; }

        public ShelfViewModel(ObservableCollection<Rental> rentals)
        {
            Rentals = rentals;

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
            DeleteShelfTypeCommand = new RelayCommand(DeleteShelfType, CanDeleteShelfType);
            AddShelfCommand = new RelayCommand(AddShelf, CanAddShelf);
            DeleteShelfCommand = new RelayCommand(DeleteShelf, CanDeleteShelf);

            // Refreshes the list of visible shelves whenever the Shelves or Rentals collections change
            Shelves.CollectionChanged += (_, _) => ApplyShelfFilter();
            Rentals.CollectionChanged += (_, _) => ApplyShelfFilter();

            ApplyShelfFilter();
        }

        private bool CanDeleteShelf(object? parameter)
        {
            return SelectedShelfRow is not null;
        }

        private void DeleteShelf(object? parameter)
        {
            if (SelectedShelfRow is null)
            {
                return;
            }

            Shelves.Remove(SelectedShelfRow.Shelf);
            SelectedShelfRow = null;
        }

        private bool CanAddShelf(object? parameter)
        {
            return int.TryParse(NewShelfNumber, out int shelfNumber)
                && shelfNumber > 0
                && NewShelfType is not null;
        }

        private int _nextShelfId = 3;
        private void AddShelf(object? parameter)
        {
            if (!int.TryParse(NewShelfNumber, out int shelfNumber) ||
                shelfNumber <= 0 ||
                NewShelfType is null)
            {
                return;
            }

            foreach (var shelf in Shelves)
            {
                if (shelf.ShelfNumber == shelfNumber)
                {
                    ShelfMessage = "Reolnummeret findes allerede.";
                    return;
                }
            }

            Shelf newShelf = new Shelf(NewShelfType)
            {
                ShelfId = _nextShelfId,
                ShelfNumber = shelfNumber
            };

            Shelves.Add(newShelf);
            _nextShelfId++;

            NewShelfNumber = string.Empty;
            ShelfMessage = string.Empty;
            NewShelfType = null;
        }

        private void ValidateShelfNumber()
        {
            if (string.IsNullOrWhiteSpace(NewShelfNumber))
            {
                ShelfMessage = string.Empty;
            }
            else if (!int.TryParse(NewShelfNumber, out int number)
                     || number <= 0)
            {
                ShelfMessage = "Reolnummeret skal være et helt tal større end 0.";
            }
            else
            {
                ShelfMessage = string.Empty;
            }
        }

        private bool CanDeleteShelfType(object? parameter)
        {
            return ShelfTypeToDelete is not null;
        }
        private void DeleteShelfType(object? parameter)
        {
            if (ShelfTypeToDelete is null)
            {
                return;
            }

            foreach (var shelf in Shelves)
            {
                if (shelf.ShelfType == ShelfTypeToDelete)
                {
                    ShelfTypeMessage = "Reoltypen bruges af en reol og kan ikke slettes.";
                    return;
                }

            }
            ShelfTypes.Remove(ShelfTypeToDelete);
            // clear the selection after deletion
            ShelfTypeToDelete = null;
            ShelfTypeMessage = string.Empty;
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

        private void ApplyShelfFilter()
        {
            SelectedShelfRow = null;
            VisibleShelves.Clear();

            var dateToday = DateTime.Today;

            foreach (var shelf in Shelves)
            {
                var currentRental =
                    _rentalService.GetCurrentRentalByShelfAndDate(shelf, dateToday, Rentals);
                var status =
                    _rentalService.GetShelfStatus(shelf, dateToday, Rentals);

                // Determine if the shelf matches the selected filter
                bool matchesFilter =
                    SelectedStatusFilter == ShelfStatusFilter.All ||
                    (SelectedStatusFilter == ShelfStatusFilter.Available &&
                     status == ShelfStatus.Available) ||
                    (SelectedStatusFilter == ShelfStatusFilter.Rented &&
                     status == ShelfStatus.Rented) ||
                     (SelectedStatusFilter == ShelfStatusFilter.TerminationPending &&
                     status == ShelfStatus.TerminationPending);

                if (matchesFilter)
                {
                    VisibleShelves.Add(new ShelfRowViewModel(shelf, currentRental, status));
                }
            }
        }

        public void Refresh()
        {
            ApplyShelfFilter();
        }
    }
}
