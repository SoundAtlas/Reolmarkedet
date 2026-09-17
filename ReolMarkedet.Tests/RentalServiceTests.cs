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

    [TestMethod]
    public void IsShelfAvailable_WhenExistingRentalHasNoEnd_ReturnsFalse()
    {
        // Arrange
        RentalService rentalService = new();
        Tenant tenant = new() { TenantId = 1, Name = "Test Tenant" };
        ShelfType shelfType = new() { ShelfTypeId = 1, Name = "Test Type" };
        Shelf shelf = new(shelfType) { ShelfId = 1, ShelfNumber = 1 };

        Rental existingRental = new(tenant, shelf)
        {
            StartDate = new DateTime(2026, 9, 1),
            EndDate = null
        };

        // Act
        bool available = rentalService.IsShelfAvailable(
            shelf,
            new DateTime(2026, 10, 1),
            new DateTime(2026, 10, 30),
            new List<Rental> { existingRental });

        // Assert
        Assert.IsFalse(available);
    }

    [TestMethod]
    public void IsShelfAvailable_WhenOpenEndedRequestOverlapsFutureRental_ReturnsFalse()
    {
        // Arrange
        RentalService rentalService = new();
        Tenant tenant = new() { TenantId = 1, Name = "Test Tenant" };
        ShelfType shelfType = new() { ShelfTypeId = 1, Name = "Test Type" };
        Shelf shelf = new(shelfType) { ShelfId = 1, ShelfNumber = 1 };

        Rental existingRental = new(tenant, shelf)
        {
            StartDate = new DateTime(2026, 10, 1),
            EndDate = new DateTime(2026, 10, 30)
        };

        // Act
        bool available = rentalService.IsShelfAvailable(
            shelf,
            new DateTime(2026, 9, 1),
            null,
            new List<Rental> { existingRental });

        // Assert
        Assert.IsFalse(available);
    }

    [TestMethod]
    public void GetShelfStatus_WhenCurrentRentalHasEndDateButNoNotice_ReturnsRented()
    {
        // Arrange
        RentalService rentalService = new();
        Tenant tenant = new() { TenantId = 1, Name = "Test Tenant" };
        ShelfType shelfType = new() { ShelfTypeId = 1, Name = "Test Type" };
        Shelf shelf = new(shelfType) { ShelfId = 1, ShelfNumber = 1 };

        Rental existingRental = new(tenant, shelf)
        {
            StartDate = new DateTime(2026, 10, 1),
            EndDate = new DateTime(2026, 10, 30),
            TerminationNoticeDate = null
        };

        List<Rental> rentals = new() { existingRental };

        // Act
        ShelfStatus status =
            rentalService.GetShelfStatus(shelf, new DateTime(2026, 10, 15), rentals);

        // Assert
        Assert.AreEqual(ShelfStatus.Rented, status);
    }

    [TestMethod]
    public void GetShelfStatus_WhenCurrentRentalHasTerminationNotice_ReturnsTerminationPending()
    {
        // Arrange
        RentalService rentalService = new();
        Tenant tenant = new() { TenantId = 1, Name = "Test Tenant" };
        ShelfType shelfType = new() { ShelfTypeId = 1, Name = "Test Type" };
        Shelf shelf = new(shelfType) { ShelfId = 1, ShelfNumber = 1 };

        Rental existingRental = new(tenant, shelf)
        {
            StartDate = new DateTime(2026, 10, 1),
            EndDate = new DateTime(2026, 10, 30),
            TerminationNoticeDate = new DateTime(2026, 10, 10)
        };

        List<Rental> rentals = new() { existingRental };

        // Act
        ShelfStatus status =
            rentalService.GetShelfStatus(shelf, new DateTime(2026, 10, 15), rentals);

        // Assert
        Assert.AreEqual(ShelfStatus.TerminationPending, status);
    }

    [TestMethod]
    public void GetShelfStatus_WhenCurrentRentalEnded_ReturnsAvailable()
    {
        // Arrange
        RentalService rentalService = new();
        Tenant tenant = new() { TenantId = 1, Name = "Test Tenant" };
        ShelfType shelfType = new() { ShelfTypeId = 1, Name = "Test Type" };
        Shelf shelf = new(shelfType) { ShelfId = 1, ShelfNumber = 1 };

        Rental existingRental = new(tenant, shelf)
        {
            StartDate = new DateTime(2026, 10, 1),
            EndDate = new DateTime(2026, 10, 14),
        };

        List<Rental> rentals = new() { existingRental };

        // Act
        ShelfStatus status =
            rentalService.GetShelfStatus(shelf, new DateTime(2026, 10, 15), rentals);

        // Assert
        Assert.AreEqual(ShelfStatus.Available, status);
    }
}
