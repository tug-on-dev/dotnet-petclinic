using Microsoft.Playwright;
using Xunit;

namespace PetClinic.Tests;

public abstract class BaseTest : IAsyncLifetime
{
    protected IPlaywright? Playwright { get; private set; }
    protected IBrowser? Browser { get; private set; }
    protected IBrowserContext? Context { get; private set; }
    protected IPage? Page { get; private set; }

    public const string JavaAppUrl = "http://localhost:8080";
    public const string DotNetAppUrl = "http://localhost:5000";

    public virtual async Task InitializeAsync()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        Context = await Browser.NewContextAsync();
        Page = await Context.NewPageAsync();
    }

    public virtual async Task DisposeAsync()
    {
        if (Page != null) await Page.CloseAsync();
        if (Context != null) await Context.CloseAsync();
        if (Browser != null) await Browser.CloseAsync();
        Playwright?.Dispose();
    }

    protected async Task NavigateToUrl(string url)
    {
        await Page!.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
    }

    protected async Task<bool> IsElementVisible(string selector, int timeoutMs = 5000)
    {
        try
        {
            await Page!.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions 
            { 
                State = WaitForSelectorState.Visible, 
                Timeout = timeoutMs 
            });
            return true;
        }
        catch
        {
            return false;
        }
    }

    protected async Task WaitForNavigation(Func<Task> action)
    {
        await Page!.RunAndWaitForNavigationAsync(action, new PageRunAndWaitForNavigationOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle
        });
    }

    protected async Task FillFormField(string selector, string value)
    {
        await Page!.FillAsync(selector, value);
    }

    protected async Task ClickButton(string selector)
    {
        await Page!.ClickAsync(selector);
    }

    protected async Task<string> GetTextContent(string selector)
    {
        return await Page!.TextContentAsync(selector) ?? "";
    }

    protected async Task<int> GetElementCount(string selector)
    {
        return await Page!.Locator(selector).CountAsync();
    }

    protected async Task<bool> HasFlashMessage(string message)
    {
        var flashSelector = ".alert, .flash-message, [class*='alert'], [class*='message']";
        try
        {
            var flashElement = await Page!.WaitForSelectorAsync(flashSelector, 
                new PageWaitForSelectorOptions { Timeout = 3000 });
            if (flashElement != null)
            {
                var text = await flashElement.TextContentAsync();
                return text?.Contains(message, StringComparison.OrdinalIgnoreCase) ?? false;
            }
        }
        catch { }
        return false;
    }

    protected async Task SearchOwnerByLastName(string lastName)
    {
        var baseUrl = GetCurrentBaseUrl();
        await Page!.GotoAsync(GetOwnersFindUrl(baseUrl), 
            new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
        await Page.FillAsync("input[name='lastName'], input[id*='lastName'], input[placeholder*='Last Name']", lastName);
        await Page.ClickAsync("button[type='submit']");
    }

    protected async Task<string> GetCurrentUrl()
    {
        return Page!.Url;
    }

    protected string GetCurrentBaseUrl()
    {
        var url = Page!.Url;
        if (url.StartsWith(JavaAppUrl)) return JavaAppUrl;
        if (url.StartsWith(DotNetAppUrl)) return DotNetAppUrl;
        return JavaAppUrl;
    }

    protected string GetVetsUrl(string baseUrl)
    {
        // Java uses lowercase /vets, .NET uses /Vet
        return baseUrl == DotNetAppUrl ? $"{baseUrl}/Vet" : $"{baseUrl}/vets";
    }

    protected string GetOwnersFindUrl(string baseUrl)
    {
        // Java uses lowercase /owners/find, .NET uses /Owner/Find
        return baseUrl == DotNetAppUrl ? $"{baseUrl}/Owner/Find" : $"{baseUrl}/owners/find";
    }

    protected string GetOwnersUrl(string baseUrl)
    {
        // Java uses lowercase /owners, .NET uses /Owner
        return baseUrl == DotNetAppUrl ? $"{baseUrl}/Owner" : $"{baseUrl}/owners";
    }
}
