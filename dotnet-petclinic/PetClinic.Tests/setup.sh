#!/bin/bash

# PetClinic E2E Test Setup Script
# This script installs Playwright browsers needed for E2E testing

echo "========================================="
echo "PetClinic E2E Test Setup"
echo "========================================="
echo ""

# Check if we're in the correct directory
if [ ! -f "PetClinic.Tests.csproj" ]; then
    echo "Error: Please run this script from the PetClinic.Tests directory"
    exit 1
fi

# Build the project
echo "Building test project..."
dotnet build

if [ $? -ne 0 ]; then
    echo "Error: Build failed"
    exit 1
fi

echo ""
echo "Build successful!"
echo ""

# Install Playwright browsers
echo "Installing Playwright Chromium browser..."
echo ""

# Try using pwsh if available
if command -v pwsh &> /dev/null; then
    echo "Using PowerShell Core..."
    pwsh bin/Debug/net10.0/playwright.ps1 install chromium
elif command -v powershell &> /dev/null; then
    echo "Using PowerShell..."
    powershell bin/Debug/net10.0/playwright.ps1 install chromium
else
    echo "PowerShell not found. Trying alternative installation method..."
    echo ""
    echo "Please install Playwright browsers manually:"
    echo ""
    echo "Option 1 - Install PowerShell:"
    echo "  brew install --cask powershell"
    echo "  pwsh bin/Debug/net10.0/playwright.ps1 install chromium"
    echo ""
    echo "Option 2 - Use npx (requires Node.js):"
    echo "  npx playwright install chromium"
    echo ""
    exit 1
fi

if [ $? -eq 0 ]; then
    echo ""
    echo "========================================="
    echo "Setup Complete!"
    echo "========================================="
    echo ""
    echo "To run tests, ensure both applications are running:"
    echo "  - Java PetClinic: http://localhost:8080"
    echo "  - .NET PetClinic: http://localhost:5000"
    echo ""
    echo "Then run: dotnet test"
    echo ""
else
    echo ""
    echo "Error: Playwright browser installation failed"
    exit 1
fi
