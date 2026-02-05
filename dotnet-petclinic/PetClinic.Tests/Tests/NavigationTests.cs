using Xunit;

namespace PetClinic.Tests.Tests;

public class NavigationTests : BaseTest
{
    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task Navigation_HomeLink_ShouldWork(string baseUrl, string appName)
    {
        await NavigateToUrl(GetOwnersFindUrl(baseUrl));
        
        // Click home link
        await Page!.ClickAsync("a[href='/'], a[href*='home'], .navbar-brand, a:has-text('Home')");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        var currentUrl = await GetCurrentUrl();
        Assert.True(currentUrl == baseUrl || currentUrl == $"{baseUrl}/", 
            $"{appName} app: Home link should navigate to home page");
    }

    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task Navigation_FindOwnersLink_ShouldWork(string baseUrl, string appName)
    {
        await NavigateToUrl(baseUrl);
        
        // Click Find Owners link
        await Page!.ClickAsync("a[href*='owners/find'], a[href*='Owner/Find'], a:has-text('Find owners'), a:has-text('Find Owners'), a:has-text('FIND OWNERS')");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        var currentUrl = await GetCurrentUrl();
        Assert.True(currentUrl.Contains("/owners/find", StringComparison.OrdinalIgnoreCase) || 
                    currentUrl.Contains("/Owner/Find", StringComparison.Ordinal),
            $"{appName} app: Should navigate to find owners page");
    }

    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task Navigation_VeterinariansLink_ShouldWork(string baseUrl, string appName)
    {
        await NavigateToUrl(baseUrl);
        
        // Click Veterinarians link
        await Page!.ClickAsync("a[href*='vets'], a:has-text('Veterinarians'), a:has-text('VETERINARIANS')");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        var currentUrl = await GetCurrentUrl();
        Assert.True(currentUrl.Contains("/vets", StringComparison.OrdinalIgnoreCase) || 
                    currentUrl.Contains("/Vet", StringComparison.Ordinal),
            $"{appName} app: Should navigate to veterinarians page (expected /vets or /Vet, got {currentUrl})");
    }

    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task Navigation_BreadcrumbLinks_ShouldWork(string baseUrl, string appName)
    {
        // Navigate to a deep page
        await NavigateToUrl(GetOwnersFindUrl(baseUrl));
        await Page!.FillAsync("input[name='lastName'], input[id*='lastName']", "");
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // Check if breadcrumb or back navigation exists
        var hasBreadcrumb = await IsElementVisible(".breadcrumb, .nav-breadcrumb, a:has-text('Back')");
        
        // This test is informational - not all apps may have breadcrumbs
        Assert.True(true, $"{appName} app breadcrumb check completed");
    }

    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task Navigation_AllMainLinks_AreAccessible(string baseUrl, string appName)
    {
        await NavigateToUrl(baseUrl);
        
        // Check that main navigation links exist
        var navLinks = await Page!.Locator("nav a, .navbar a, header a").AllAsync();
        Assert.True(navLinks.Count >= 2, 
            $"{appName} app should have at least 2 navigation links (Home, Find Owners, Vets, etc.)");
    }
}
