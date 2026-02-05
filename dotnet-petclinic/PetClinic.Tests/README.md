# PetClinic E2E Tests

Comprehensive end-to-end testing suite for PetClinic applications using xUnit and Playwright.

## Overview

This test project validates feature parity between the **Java Spring PetClinic** (port 8080) and **.NET PetClinic** (port 5000) applications. All tests are parameterized to run against both applications using xUnit's `[Theory]` and `[InlineData]` attributes.

## Prerequisites

- .NET 10 SDK or later
- Playwright browsers (Chromium)
- Both PetClinic applications running:
  - Java app: http://localhost:8080
  - .NET app: http://localhost:5000

## Installation

### 1. Install Playwright Browsers

```bash
# Build the project first
cd PetClinic.Tests
dotnet build

# Install Playwright browsers using PowerShell (if available)
pwsh bin/Debug/net10.0/playwright.ps1 install chromium

# Or on Linux/Mac with PowerShell
pwsh bin/Debug/net10.0/playwright.ps1 install chromium

# Alternative: Install using .NET tool
dotnet tool install --global Microsoft.Playwright.CLI
~/.dotnet/tools/playwright install chromium
```

## Running Tests

### Run All Tests

```bash
dotnet test
```

### Run Specific Test Class

```bash
dotnet test --filter "FullyQualifiedName~HomeTests"
dotnet test --filter "FullyQualifiedName~NavigationTests"
dotnet test --filter "FullyQualifiedName~OwnerSearchTests"
dotnet test --filter "FullyQualifiedName~OwnerCrudTests"
dotnet test --filter "FullyQualifiedName~OwnerPaginationTests"
dotnet test --filter "FullyQualifiedName~PetCrudTests"
dotnet test --filter "FullyQualifiedName~VisitTests"
dotnet test --filter "FullyQualifiedName~VetListTests"
```

### Run Tests for Specific App

```bash
# Java app only
dotnet test --filter "DisplayName~Java"

# .NET app only
dotnet test --filter "DisplayName~.NET"
```

### Run with Verbose Output

```bash
dotnet test -v normal
```

## Test Structure

### BaseTest.cs
Base class providing:
- Browser initialization (Chromium headless)
- Page lifecycle management
- Helper methods for common operations
- Constants for both app URLs

### Test Suites

#### HomeTests.cs
- Home page loads successfully
- Welcome image displays
- Navigation menu is present

#### NavigationTests.cs
- Home link navigation
- Find Owners link navigation
- Veterinarians link navigation
- All main navigation links are accessible

#### OwnerSearchTests.cs
- Empty search returns all owners
- Partial last name search
- Single result redirects to details
- Multiple results show list
- No results show appropriate message
- Case-insensitive search

#### OwnerCrudTests.cs
- Create owner with valid data
- Validation for missing required fields
- Validation for invalid telephone
- Edit owner updates details
- Owner details display all information

#### OwnerPaginationTests.cs
- Pagination controls display
- Maximum items per page (5 items)
- Next page navigation
- Previous page navigation
- Page numbers are clickable
- Current page is highlighted

#### PetCrudTests.cs
- Add pet with valid data
- Validation for missing name
- Validation for invalid birth date (future)
- Edit pet updates details
- Owner details display pets list
- Pet type dropdown has options

#### VisitTests.cs
- Add visit with valid data
- Validation for missing description
- Owner details display visits history
- Visit form displays pet information
- Visit date is required
- Visits list shows date and description

#### VetListTests.cs
- Vet list displays successfully
- Display vet names
- Display specialties
- Table headers present
- Pagination works (if present)
- Multiple vets displayed
- Page loads quickly
- Navigation from home works
- Handles vets with no specialties
- Displays vets with multiple specialties

## Test Methodology

### Parameterized Testing
Each test uses `[Theory]` with `[InlineData]` to run against both apps:

```csharp
[Theory]
[InlineData(JavaAppUrl, "Java")]
[InlineData(DotNetAppUrl, ".NET")]
public async Task TestName(string baseUrl, string appName)
{
    // Test implementation
}
```

### Flexible Selectors
Tests use multiple selector strategies to accommodate different implementations:
- CSS selectors
- Text content matching
- Multiple fallback options

### Graceful Degradation
Tests handle variations in implementation:
- Optional features (pagination, breadcrumbs)
- Different DOM structures
- Varying validation approaches

## Expected Test Results

With both applications running, all tests should pass, demonstrating feature parity between:
- ✅ Core navigation
- ✅ Search functionality
- ✅ CRUD operations (Owners, Pets, Visits)
- ✅ Form validation
- ✅ Pagination
- ✅ Data display

## Troubleshooting

### Browsers Not Installed
```bash
pwsh bin/Debug/net10.0/playwright.ps1 install
```

### Applications Not Running
Ensure both apps are running before tests:
```bash
# Terminal 1: Start Java app
cd spring-petclinic-main
./mvnw spring-boot:run

# Terminal 2: Start .NET app
cd dotnet-petclinic
dotnet run
```

### Test Timeouts
Increase timeout in `BaseTest.cs` if needed:
```csharp
await Page!.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions 
{ 
    Timeout = 10000  // Increase from 5000 to 10000
});
```

### Headless vs Headed Mode
To see browser during test execution, edit `BaseTest.cs`:
```csharp
Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
{
    Headless = false  // Change to false
});
```

## CI/CD Integration

### GitHub Actions Example
```yaml
- name: Install Playwright
  run: |
    dotnet build PetClinic.Tests
    pwsh PetClinic.Tests/bin/Debug/net10.0/playwright.ps1 install chromium

- name: Start Applications
  run: |
    # Start Java app in background
    # Start .NET app in background
    
- name: Run Tests
  run: dotnet test PetClinic.Tests --logger "trx"
```

## Contributing

When adding new tests:
1. Extend `BaseTest` if adding common functionality
2. Use `[Theory]` with both app URLs
3. Use flexible selectors for compatibility
4. Add descriptive assertion messages
5. Handle optional features gracefully

## License

Same as parent PetClinic project.
