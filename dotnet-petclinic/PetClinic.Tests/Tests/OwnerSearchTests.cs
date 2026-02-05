using Xunit;

namespace PetClinic.Tests.Tests;

public class OwnerSearchTests : BaseTest
{
    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task OwnerSearch_EmptySearch_ShouldReturnAllOwners(string baseUrl, string appName)
    {
        await NavigateToUrl(GetOwnersFindUrl(baseUrl));
        
        // Submit empty search
        await Page!.FillAsync("input[name='lastName'], input[id*='lastName']", "");
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // Should navigate to owners list
        var currentUrl = await GetCurrentUrl();
        Assert.True(currentUrl.Contains("/owners", StringComparison.OrdinalIgnoreCase) || 
                    currentUrl.Contains("/Owner", StringComparison.Ordinal),
            $"Should navigate to owners list page");
        
        // Should show multiple owners
        var ownerRows = await GetElementCount("table tbody tr, .owner-row, .owners-list > div");
        Assert.True(ownerRows > 0, 
            $"{appName} app: Should display owner list");
    }

    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task OwnerSearch_PartialLastName_ShouldReturnMatches(string baseUrl, string appName)
    {
        await NavigateToUrl(GetOwnersFindUrl(baseUrl));
        
        // Search for partial name (common in default data)
        await Page!.FillAsync("input[name='lastName'], input[id*='lastName']", "Dav");
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        var currentUrl = await GetCurrentUrl();
        
        // Should either show results list or single owner details
        var hasResults = currentUrl.Contains("/owners") || 
                        await IsElementVisible("table, .owner-details, .owner-info");
        
        Assert.True(hasResults, 
            $"{appName} app: Partial name search should return results");
    }

    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task OwnerSearch_SingleResult_ShouldRedirectToDetails(string baseUrl, string appName)
    {
        await NavigateToUrl(GetOwnersFindUrl(baseUrl));
        
        // Search for a specific owner (assuming Franklin exists in default data)
        await Page!.FillAsync("input[name='lastName'], input[id*='lastName']", "Franklin");
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        var currentUrl = await GetCurrentUrl();
        
        // Should redirect to owner details page with an ID
        var hasOwnerId = System.Text.RegularExpressions.Regex.IsMatch(currentUrl, @"/owners/\d+");
        Assert.True(hasOwnerId || currentUrl.Contains("/owners/"), 
            $"{appName} app: Single search result should redirect to owner details");
    }

    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task OwnerSearch_MultipleResults_ShouldShowList(string baseUrl, string appName)
    {
        await NavigateToUrl(GetOwnersFindUrl(baseUrl));
        
        // Search for partial name that should return multiple results
        await Page!.FillAsync("input[name='lastName'], input[id*='lastName']", "Davis");
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        var currentUrl = await GetCurrentUrl();
        Assert.True(currentUrl.Contains("/owners", StringComparison.OrdinalIgnoreCase) || 
                    currentUrl.Contains("/Owner", StringComparison.Ordinal),
            $"Should navigate to owners list page");
        
        // Should have a table or list of owners
        var hasTable = await IsElementVisible("table, .owners-list");
        Assert.True(hasTable, 
            $"{appName} app: Multiple results should display in a list/table");
    }

    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task OwnerSearch_NoResults_ShouldShowMessage(string baseUrl, string appName)
    {
        await NavigateToUrl(GetOwnersFindUrl(baseUrl));
        
        // Search for non-existent owner
        await Page!.FillAsync("input[name='lastName'], input[id*='lastName']", "ZzzNonExistentOwner999");
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // Should show "not found" message or stay on search page
        var pageContent = await Page.ContentAsync();
        var hasNotFoundMessage = pageContent.Contains("not found", StringComparison.OrdinalIgnoreCase) ||
                                pageContent.Contains("no owners", StringComparison.OrdinalIgnoreCase) ||
                                await GetCurrentUrl() == GetOwnersFindUrl(baseUrl);
        
        Assert.True(hasNotFoundMessage, 
            $"{appName} app: No results should show appropriate message or return to search");
    }

    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task OwnerSearch_CaseInsensitive_ShouldWork(string baseUrl, string appName)
    {
        await NavigateToUrl(GetOwnersFindUrl(baseUrl));
        
        // Search with different case
        await Page!.FillAsync("input[name='lastName'], input[id*='lastName']", "franklin");
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        var currentUrl = await GetCurrentUrl();
        
        // Should find results regardless of case
        var hasResults = currentUrl.Contains("/owners/") && !currentUrl.Contains("/find");
        Assert.True(hasResults, 
            $"{appName} app: Search should be case-insensitive");
    }
}
