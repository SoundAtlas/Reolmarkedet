using Reolmarkedet.Core.Interfaces;
using Reolmarkedet.Core.Models;
using Reolmarkedet.Core.Services;

namespace ReolMarkedet.Tests;

[TestClass]
public class SalesServiceTests
{
    [TestMethod]
    public async Task RegisterSaleAsync_WhenBarcodeDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange: Create fake repositories
        IItemRepository itemRepository = new FakeItemRepository();
        ISaleRepository saleRepository = new FakeSaleRepository();

        SalesService salesService = new(itemRepository, saleRepository);

        // Act and Assert: call RegisterSaleAsync and expect
        // an InvalidOperationException.
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => salesService.RegisterSaleAsync(
                "UNKNOWN",
                100.00m,
               new DateOnly(2026, 9, 15)));
    }

    [TestMethod]
    public async Task RegisterSaleAsync_WhenItemWasAlreadySold_ThrowsInvalidOperationException()
    {
        FakeItemRepository itemRepository = new();
        itemRepository.ItemToReturn = new Item
        {
            ItemId = 42,
            Barcode = "ABC123"
        };

        FakeSaleRepository saleRepository = new();
        saleRepository.ExistsForItemResult = true;

        SalesService salesService = new(itemRepository, saleRepository);

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => salesService.RegisterSaleAsync(
                "ABC123",
                100.00m,
                new DateOnly(2026, 9, 15)));
        Assert.IsFalse(saleRepository.AddWasCalled);
    }

    [TestMethod]
    public async Task RegisterSaleAsync_WhenSaleIsValid_ReturnsGeneratedSaleId()
    {
        DateOnly saleDate = new(2026, 9, 15);

        FakeItemRepository itemRepository = new();
        itemRepository.ItemToReturn = new Item
        {
            ItemId = 42,
            Barcode = "ABC123"
        };

        FakeSaleRepository saleRepository = new();
        saleRepository.ExistsForItemResult = false;
        saleRepository.AddResult = 17;

        SalesService salesService = new(itemRepository, saleRepository);

        int generatedSaleId = await salesService.RegisterSaleAsync(
            "ABC123",
            125.50m,
            saleDate);

        Assert.AreEqual(17, generatedSaleId);
        Assert.IsNotNull(saleRepository.AddedSale);

        Sale addedSale = saleRepository.AddedSale!;

        Assert.AreEqual(42, addedSale.ItemId);
        Assert.AreEqual(125.50m, addedSale.SalePrice);
        Assert.AreEqual(saleDate, addedSale.SaleDate);
        Assert.IsTrue(saleRepository.AddWasCalled);
    }

    private sealed class FakeItemRepository : IItemRepository
    {
        public Item? ItemToReturn { get; set; }
        public Task<Item?> GetByBarcodeAsync(string barcode)
        {
            // Simulate an unknown barcode by returning a Task
            // whose Item result is null.
            return Task.FromResult<Item?>(ItemToReturn);
        }
    }

    private sealed class FakeSaleRepository : ISaleRepository
    {
        public bool ExistsForItemResult { get; set; }
        public bool AddWasCalled { get; private set; }
        public int AddResult { get; set; }
        public Sale? AddedSale { get; private set; }
        public Task<bool> ExistsForItemAsync(int itemId)
        {
            return Task.FromResult(ExistsForItemResult);
        }

        public Task<int> AddAsync(Sale sale)
        {
            AddWasCalled = true;
            AddedSale = sale;
            return Task.FromResult(AddResult);
        }
    }
}
