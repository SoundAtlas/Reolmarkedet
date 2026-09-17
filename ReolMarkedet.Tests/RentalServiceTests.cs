using Reolmarkedet.Core.Models;
using Reolmarkedet.Core.Services;

namespace ReolMarkedet.Tests;

[TestClass]
public class RentalServiceTests
{
    [TestMethod]
    public void IsShelfAvailable_WhenPeriodsShareBoundaryDay_ReturnsFalse()
    {
        // Arrange
        RentalService rentalService = new();
        Tenant tenant = new() { TenantId = 1, Name = "Test Tenant" };
        ShelfType shelfType = new() { ShelfTypeId = 1, Name = "Test Type" };
        Shelf shelf = new(shelfType) { ShelfId = 1, ShelfNumber = 1 };

        Rental existingRental = new(tenant, shelf)
        {
            StartDate = new DateTime(2026, 9, 1),
            EndDate = new DateTime(2026, 9, 30)
        };

        // Act
        bool available = rentalService.IsShelfAvailable(
            shelf,
            new DateTime(2026, 9, 30),
            new DateTime(2026, 10, 30),
            new List<Rental> { existingRental });

        // Assert
        Assert.IsFalse(available);
    }

    [TestMethod]
    public void IsShelfAvailable_WhenRequestedPeriodStartsAfterExistingEnd_ReturnsTrue()
    {
        // Arrange
        RentalService rentalService = new();
        Tenant tenant = new() { TenantId = 1, Name = "Test Tenant" };
        ShelfType shelfType = new() { ShelfTypeId = 1, Name = "Test Type" };
        Shelf shelf = new(shelfType) { ShelfId = 1, ShelfNumber = 1 };

        Rental existingRental = new(tenant, shelf)
        {
            StartDate = new DateTime(2026, 9, 1),
            EndDate = new DateTime(2026, 9, 30)
        };

        // Act
        bool available = rentalService.IsShelfAvailable(
            shelf,
            new DateTime(2026, 10, 1),
            new DateTime(2026, 10, 30),
            new List<Rental> { existingRental });

        // Assert
        Assert.IsTrue(available);
    }
}
