# 🚀 PetClinic E2E Tests - Quick Reference

## One-Time Setup
```bash
cd PetClinic.Tests
./setup.sh
```

## Run Tests

### All Tests
```bash
dotnet test
```

### Specific Test Suite
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

### By Application
```bash
dotnet test --filter "DisplayName~Java"   # Java app only
dotnet test --filter "DisplayName~.NET"   # .NET app only
```

### With Details
```bash
dotnet test -v normal        # Normal verbosity
dotnet test -v detailed      # Detailed output
dotnet test --logger "console;verbosity=detailed"
```

## Prerequisites Checklist

- [ ] .NET 10 SDK installed
- [ ] Playwright browsers installed (`./setup.sh`)
- [ ] Java app running on http://localhost:8080
- [ ] .NET app running on http://localhost:5000

## Quick App Startup

### Terminal 1: Java App
```bash
cd spring-petclinic-main
./mvnw spring-boot:run
```

### Terminal 2: .NET App
```bash
cd dotnet-petclinic
dotnet run
```

### Terminal 3: Run Tests
```bash
cd dotnet-petclinic/PetClinic.Tests
dotnet test
```

## Test Structure

```
PetClinic.Tests/
├── BaseTest.cs              # Base class with helpers
├── Tests/
│   ├── HomeTests.cs         # 3 tests - Home page
│   ├── NavigationTests.cs   # 5 tests - Navigation
│   ├── OwnerSearchTests.cs  # 6 tests - Search functionality
│   ├── OwnerCrudTests.cs    # 5 tests - Owner CRUD
│   ├── OwnerPaginationTests.cs  # 6 tests - Pagination
│   ├── PetCrudTests.cs      # 6 tests - Pet CRUD
│   ├── VisitTests.cs        # 6 tests - Visit management
│   └── VetListTests.cs      # 10 tests - Vet list
├── README.md                # Full documentation
├── TEST_SUMMARY.md          # Comprehensive summary
└── setup.sh                 # Setup script
```

## Common Issues

### "Browser not installed"
```bash
pwsh bin/Debug/net10.0/playwright.ps1 install chromium
```

### "Connection refused"
- Ensure both apps are running
- Check ports 8080 and 5000 are not in use by other apps

### Tests timing out
- Increase timeouts in `BaseTest.cs`
- Check app performance

### Tests failing on one app but not the other
- **This is the point!** Tests reveal feature parity issues

## Debug Mode

To see browser during tests:
1. Edit `BaseTest.cs`
2. Change `Headless = true` to `Headless = false`
3. Rebuild and run tests

## CI/CD Integration

```yaml
# Example GitHub Actions
- name: Setup
  run: ./PetClinic.Tests/setup.sh

- name: Start Apps
  run: |
    cd spring-petclinic-main && ./mvnw spring-boot:run &
    cd dotnet-petclinic && dotnet run &
    sleep 30

- name: Run Tests
  run: dotnet test PetClinic.Tests
```

## Key Metrics

- **94 total test executions** (47 tests × 2 apps)
- **8 test suites** covering all major features
- **Chromium browser** in headless mode
- **~3-5 minutes** typical run time (depending on hardware)

## What Tests Validate

✅ Navigation works on both apps  
✅ Search functionality matches  
✅ CRUD operations have feature parity  
✅ Form validation is consistent  
✅ Pagination behaves the same  
✅ Data displays correctly  
✅ Error handling is equivalent  

## Next Steps

1. Run setup script
2. Start both applications
3. Run `dotnet test`
4. Review results
5. Fix any failures in the app with issues
6. Iterate until all tests pass on both apps

---

📚 **Full docs**: See `README.md` and `TEST_SUMMARY.md`  
🐛 **Issues**: Check test output and app logs  
🔧 **Customize**: Edit tests or add new ones in `Tests/` directory
