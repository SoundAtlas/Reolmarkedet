using Reolmarkedet.Core.Models;
using Reolmarkedet.WPF.ViewModels;
using System.Collections.ObjectModel;

namespace ReolMarkedet.Tests;

[TestClass]
public class ShelfViewModelTests
{
    [TestMethod]
    public void DeleteShelfType_WhenTypeIsUsed_LeavesTypeUnchanged()
    {
        // Arrange
        var viewModel = new ShelfViewModel(new ObservableCollection<Rental>());
        ShelfType shelfType = viewModel.Shelves[1].ShelfType;

        int originalCount = viewModel.ShelfTypes.Count;

        viewModel.ShelfTypeToDelete = shelfType;

        // Act
        viewModel.DeleteShelfTypeCommand.Execute(null);

        // Assert
        Assert.HasCount(originalCount, viewModel.ShelfTypes);
        Assert.Contains(shelfType, viewModel.ShelfTypes);
        Assert.AreEqual(shelfType, viewModel.ShelfTypeToDelete);
        Assert.IsFalse(string.IsNullOrWhiteSpace(viewModel.ShelfTypeMessage));
    }

    [TestMethod]
    public void AddShelf_WhenNumberAlreadyExists_DoesNotAddShelf()
    {
        // Arrange
        var viewModel = new ShelfViewModel(new ObservableCollection<Rental>());

        int originalCount = viewModel.Shelves.Count();

        viewModel.NewShelfNumber = "1";
        viewModel.NewShelfType = viewModel.ShelfTypes[0];

        // Act
        viewModel.AddShelfCommand.Execute(null);

        // Assert
        Assert.HasCount(originalCount, viewModel.Shelves);
        Assert.IsFalse(string.IsNullOrWhiteSpace(viewModel.ShelfMessage));
        Assert.AreEqual("1", viewModel.NewShelfNumber);
    }
}
