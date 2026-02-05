using Xunit;

namespace PetClinic.Tests.Tests;

public class HomeTests : BaseTest
{
    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task HomePage_ShouldLoad_Successfully(string baseUrl, string appName)
    {
        // Navigate to home page
        await NavigateToUrl(baseUrl);

        // Verify page title or heading
        var title = await Page!.TitleAsync();
        Assert.True(title.Contains("PetClinic", StringComparison.OrdinalIgnoreCase) || 
                    title.Contains("Pet Clinic", StringComparison.OrdinalIgnoreCase),
            $"{appName} app: Title should contain 'PetClinic' or 'Pet Clinic' (got: {title})");

        // Verify welcome message or main content is visible
        var hasWelcome = await IsElementVisible("h1, h2, .welcome");
        Assert.True(hasWelcome, $"{appName} app home page should display welcome content");
    }

    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task HomePage_ShouldDisplay_WelcomeImage(string baseUrl, string appName)
    {
        await NavigateToUrl(baseUrl);

        // Check for welcome image
        var hasImage = await IsElementVisible("img[src*='pets'], img[alt*='pet'], .welcome img", 10000);
        Assert.True(hasImage, $"{appName} app should display welcome image on home page");
    }

    [Theory]
    [InlineData(JavaAppUrl, "Java")]
    [InlineData(DotNetAppUrl, ".NET")]
    public async Task HomePage_ShouldDisplay_NavigationMenu(string baseUrl, string appName)
    {
        await NavigateToUrl(baseUrl);

        // Verify navigation menu is present
        var hasNav = await IsElementVisible("nav, .navbar, ul.nav, header");
        Assert.True(hasNav, $"{appName} app should display navigation menu");
    }
}
