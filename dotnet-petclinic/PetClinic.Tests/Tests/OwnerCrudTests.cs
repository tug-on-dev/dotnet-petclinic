using Xunit;

namespace PetClinic.Tests.Tests;

public class OwnerCrudTests : BaseTest
{
    private readonly string _testLastName = $"TestOwner{DateTime.Now.Ticks}";
    private readonly string _testFirstName = "AutoTest";

    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task CreateOwner_WithValidData_ShouldSucceed(string baseUrl, string appName)
    {
        await NavigateToUrl(GetOwnersFindUrl(baseUrl));
        
        // Click Add Owner button
        await Page!.ClickAsync("a[href*='owners/new'], button:has-text('Add Owner'), a:has-text('Add Owner')");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // Fill form
        await FillFormField("input[name='firstName'], input[id*='firstName']", _testFirstName);
        await FillFormField("input[name='lastName'], input[id*='lastName']", _testLastName);
        await FillFormField("input[name='address'], input[id*='address']", "123 Test Street");
        await FillFormField("input[name='city'], input[id*='city']", "TestCity");
        await FillFormField("input[name='telephone'], input[id*='telephone']", "1234567890");
        
        // Submit form
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        var currentUrl = await GetCurrentUrl();
        Assert.Contains("/owners/", currentUrl);
        
        var pageContent = await Page.ContentAsync();
        Assert.Contains(_testLastName, pageContent);
    }

    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task CreateOwner_WithMissingRequiredFields_ShouldShowValidation(string baseUrl, string appName)
    {
        await NavigateToUrl(GetOwnersFindUrl(baseUrl));
        
        // Click Add Owner
        await Page!.ClickAsync("a[href*='owners/new'], button:has-text('Add Owner'), a:has-text('Add Owner')");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // Leave fields empty and submit
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        var currentUrl = await GetCurrentUrl();
        
        // Should stay on form page or show validation errors
        var hasValidationError = currentUrl.Contains("/new") || 
                                await IsElementVisible(".error, .invalid-feedback, .field-validation-error, .alert-danger");
        
        Assert.True(hasValidationError, 
            $"{appName} app: Missing required fields should show validation errors");
    }

    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task CreateOwner_WithInvalidTelephone_ShouldShowValidation(string baseUrl, string appName)
    {
        await NavigateToUrl(GetOwnersFindUrl(baseUrl));
        
        // Click Add Owner
        await Page!.ClickAsync("a[href*='owners/new'], button:has-text('Add Owner'), a:has-text('Add Owner')");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // Fill with invalid phone
        await FillFormField("input[name='firstName'], input[id*='firstName']", "Test");
        await FillFormField("input[name='lastName'], input[id*='lastName']", "Invalid");
        await FillFormField("input[name='address'], input[id*='address']", "123 Street");
        await FillFormField("input[name='city'], input[id*='city']", "City");
        await FillFormField("input[name='telephone'], input[id*='telephone']", "abc");
        
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        var currentUrl = await GetCurrentUrl();
        
        // Should show validation error for telephone
        var hasError = currentUrl.Contains("/new") || 
                      await IsElementVisible(".error, .invalid-feedback, .field-validation-error");
        
        Assert.True(hasError, 
            $"{appName} app: Invalid telephone should show validation error");
    }

    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task EditOwner_ShouldUpdateDetails(string baseUrl, string appName)
    {
        // First create an owner
        await NavigateToUrl(GetOwnersFindUrl(baseUrl));
        await Page!.ClickAsync("a[href*='owners/new'], button:has-text('Add Owner'), a:has-text('Add Owner')");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        var uniqueLastName = $"EditTest{DateTime.Now.Ticks}";
        await FillFormField("input[name='firstName'], input[id*='firstName']", "Edit");
        await FillFormField("input[name='lastName'], input[id*='lastName']", uniqueLastName);
        await FillFormField("input[name='address'], input[id*='address']", "123 Old Street");
        await FillFormField("input[name='city'], input[id*='city']", "OldCity");
        await FillFormField("input[name='telephone'], input[id*='telephone']", "1111111111");
        
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // Click Edit button
        await Page.ClickAsync("a[href*='/edit'], button:has-text('Edit'), a:has-text('Edit Owner')");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // Update address
        await FillFormField("input[name='address'], input[id*='address']", "456 New Street");
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // Verify update
        var pageContent = await Page.ContentAsync();
        Assert.Contains("456 New Street", pageContent);
    }

    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task OwnerDetails_ShouldDisplayAllInformation(string baseUrl, string appName)
    {
        // Search for existing owner
        await NavigateToUrl(GetOwnersFindUrl(baseUrl));
        await Page!.FillAsync("input[name='lastName'], input[id*='lastName']", "");
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // Click on first owner
        await Page.ClickAsync("table tbody tr:first-child a, .owner-row:first-child a, a[href*='/owners/']:first-of-type");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        var currentUrl = await GetCurrentUrl();
        Assert.Contains("/owners/", currentUrl);
        
        // Verify owner information sections exist
        var hasOwnerInfo = await IsElementVisible("table, .owner-details, dl, .info");
        Assert.True(hasOwnerInfo, 
            $"{appName} app: Owner details page should display owner information");
    }
}
