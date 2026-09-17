using Reolmarkedet.Core.Models;
using Reolmarkedet.WPF.ViewModels;
using System.Collections.ObjectModel;

namespace ReolMarkedet.Tests;

[TestClass]
public class TenantViewModelTests
{
    [TestMethod]
    public void UpdateTenant_WhenNameIsBlank_LeavesTenantUnchanged()
    {
        // Arrange
        TenantViewModel viewModel = new(new ObservableCollection<Rental>());
        viewModel.Name = "John Doe";
        viewModel.AddTenantCommand.Execute(null);

        Tenant tenant = viewModel.Tenants[0];
        viewModel.SelectedTenant = tenant;
        viewModel.Name = ""; // Set name to blank

        // Act
        viewModel.UpdateTenantCommand.Execute(null);

        // Assert
        Assert.AreEqual("John Doe", tenant.Name);
        Assert.AreSame(tenant, viewModel.SelectedTenant);
        Assert.IsFalse(string.IsNullOrWhiteSpace(viewModel.ValidationMessage));
    }

    [TestMethod]
    public void CancelUpdateTenant_WhenDraftHasChanges_LeavesTenantUnchanged()
    {
        // Arrange
        TenantViewModel viewModel = new(new ObservableCollection<Rental>());
        viewModel.Name = "John Doe";
        viewModel.AddTenantCommand.Execute(null);

        Tenant tenant = viewModel.Tenants[0];
        viewModel.SelectedTenant = tenant;
        viewModel.Name = "Changed Name";

        // Act
        viewModel.CancelUpdateTenantCommand.Execute(null);

        // Assert
        Assert.AreEqual("John Doe", tenant.Name);
        Assert.IsNull(viewModel.SelectedTenant);
        Assert.IsEmpty(viewModel.Name);
    }
}
