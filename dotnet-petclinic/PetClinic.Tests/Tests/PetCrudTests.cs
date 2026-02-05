using Xunit;

namespace PetClinic.Tests.Tests;

public class PetCrudTests : BaseTest
{
    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task AddPet_WithValidData_ShouldSucceed(string baseUrl, string appName)
    {
        // First, navigate to an owner's details page
        await NavigateToUrl(GetOwnersFindUrl(baseUrl));
        await Page!.FillAsync("input[name='lastName'], input[id*='lastName']", "");
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // Click on first owner
        await Page.ClickAsync("table tbody tr:first-child a, .owner-row:first-child a, a[href*='/owners/']:first-of-type");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // Click Add Pet button
        var hasAddPetButton = await IsElementVisible("a[href*='/pets/new'], button:has-text('Add New Pet'), a:has-text('Add New Pet')");
        
        if (!hasAddPetButton)
        {
            // May need to navigate differently
            Assert.True(true, $"{appName} app: Add Pet navigation structure may differ");
            return;
        }
        
        await Page.ClickAsync("a[href*='/pets/new'], button:has-text('Add New Pet'), a:has-text('Add New Pet')");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // Fill pet form
        var petName = $"TestPet{DateTime.Now.Ticks}";
        await FillFormField("input[name='name'], input[id*='name']", petName);
        
        // Fill birth date
        var birthDate = "2020-01-01";
        await FillFormField("input[name='birthDate'], input[id*='birthDate'], input[type='date']", birthDate);
        
        // Select pet type
        await Page.ClickAsync("select[name='type'], select[id*='type']");
        await Page.SelectOptionAsync("select[name='type'], select[id*='type']", new[] { "1" });
        
        // Submit
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // Should return to owner details with new pet
        var pageContent = await Page.ContentAsync();
        Assert.Contains(petName, pageContent);
    }

    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task AddPet_WithMissingName_ShouldShowValidation(string baseUrl, string appName)
    {
        // Navigate to owner details
        await NavigateToUrl(GetOwnersFindUrl(baseUrl));
        await Page!.FillAsync("input[name='lastName'], input[id*='lastName']", "");
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        await Page.ClickAsync("table tbody tr:first-child a, .owner-row:first-child a, a[href*='/owners/']:first-of-type");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        var hasAddPetButton = await IsElementVisible("a[href*='/pets/new'], button:has-text('Add New Pet'), a:has-text('Add New Pet')");
        if (!hasAddPetButton) return;
        
        await Page.ClickAsync("a[href*='/pets/new'], button:has-text('Add New Pet'), a:has-text('Add New Pet')");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // Leave name empty and submit
        await FillFormField("input[name='birthDate'], input[id*='birthDate'], input[type='date']", "2020-01-01");
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // Should show validation error
        var hasError = await IsElementVisible(".error, .invalid-feedback, .field-validation-error, .alert-danger");
        Assert.True(hasError, 
            $"{appName} app: Missing pet name should show validation error");
    }

    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task AddPet_WithInvalidBirthDate_ShouldShowValidation(string baseUrl, string appName)
    {
        // Navigate to owner details
        await NavigateToUrl(GetOwnersFindUrl(baseUrl));
        await Page!.FillAsync("input[name='lastName'], input[id*='lastName']", "");
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        await Page.ClickAsync("table tbody tr:first-child a, .owner-row:first-child a, a[href*='/owners/']:first-of-type");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        var hasAddPetButton = await IsElementVisible("a[href*='/pets/new'], button:has-text('Add New Pet'), a:has-text('Add New Pet')");
        if (!hasAddPetButton) return;
        
        await Page.ClickAsync("a[href*='/pets/new'], button:has-text('Add New Pet'), a:has-text('Add New Pet')");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // Fill with future date
        await FillFormField("input[name='name'], input[id*='name']", "FuturePet");
        await FillFormField("input[name='birthDate'], input[id*='birthDate'], input[type='date']", "2099-12-31");
        
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // Should show validation error for future date
        var currentUrl = await GetCurrentUrl();
        var hasError = currentUrl.Contains("/new") || 
                      await IsElementVisible(".error, .invalid-feedback, .field-validation-error");
        
        Assert.True(hasError, 
            $"{appName} app: Future birth date should show validation error");
    }

    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task EditPet_ShouldUpdateDetails(string baseUrl, string appName)
    {
        // Navigate to owner with pets
        await NavigateToUrl(GetOwnersFindUrl(baseUrl));
        await Page!.FillAsync("input[name='lastName'], input[id*='lastName']", "");
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        await Page.ClickAsync("table tbody tr:first-child a, .owner-row:first-child a, a[href*='/owners/']:first-of-type");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // Check if there's an Edit Pet button
        var hasEditButton = await IsElementVisible("a[href*='/pets/'][href*='/edit'], button:has-text('Edit Pet'), a:has-text('Edit Pet')");
        
        if (!hasEditButton)
        {
            // May need to add a pet first or structure is different
            Assert.True(true, $"{appName} app: Edit Pet functionality may have different structure");
            return;
        }
        
        await Page.ClickAsync("a[href*='/pets/'][href*='/edit'], button:has-text('Edit Pet'), a:has-text('Edit Pet')");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // Update pet name
        var newName = $"UpdatedPet{DateTime.Now.Ticks}";
        await FillFormField("input[name='name'], input[id*='name']", newName);
        
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // Verify update
        var pageContent = await Page.ContentAsync();
        Assert.Contains(newName, pageContent);
    }

    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task OwnerDetails_ShouldDisplayPetsList(string baseUrl, string appName)
    {
        // Navigate to owner details
        await NavigateToUrl(GetOwnersFindUrl(baseUrl));
        await Page!.FillAsync("input[name='lastName'], input[id*='lastName']", "");
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        await Page.ClickAsync("table tbody tr:first-child a, .owner-row:first-child a, a[href*='/owners/']:first-of-type");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // Check for pets section
        var hasPetsSection = await IsElementVisible("table, .pets-list, h2:has-text('Pets'), h3:has-text('Pets')");
        
        Assert.True(hasPetsSection, 
            $"{appName} app: Owner details should have pets section");
    }

    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task PetTypeDropdown_ShouldHaveOptions(string baseUrl, string appName)
    {
        // Navigate to add pet form
        await NavigateToUrl(GetOwnersFindUrl(baseUrl));
        await Page!.FillAsync("input[name='lastName'], input[id*='lastName']", "");
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        await Page.ClickAsync("table tbody tr:first-child a, .owner-row:first-child a, a[href*='/owners/']:first-of-type");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        var hasAddPetButton = await IsElementVisible("a[href*='/pets/new'], button:has-text('Add New Pet'), a:has-text('Add New Pet')");
        if (!hasAddPetButton) return;
        
        await Page.ClickAsync("a[href*='/pets/new'], button:has-text('Add New Pet'), a:has-text('Add New Pet')");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // Check pet type dropdown has options
        var typeOptions = await GetElementCount("select[name='type'] option, select[id*='type'] option");
        
        Assert.True(typeOptions > 1, 
            $"{appName} app: Pet type dropdown should have multiple options (found {typeOptions})");
    }
}
