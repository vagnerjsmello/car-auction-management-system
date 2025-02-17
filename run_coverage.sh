#!/bin/bash
# This script installs reportgenerator if not present,
# runs tests with coverage, finds the newest folder in TestResults,
# generates an HTML report, and tries to open index.html.

# Check if reportgenerator is installed
echo "Checking if reportgenerator is installed..."
if ! command -v reportgenerator &> /dev/null; then
    echo "reportgenerator not found, installing..."
    dotnet tool install --global dotnet-reportgenerator-globaltool
else
    echo "reportgenerator is already installed."
fi

# Go to src/CAMS.Tests
echo "Changing directory to src/CAMS.Tests..."
cd src/CAMS.Tests || { echo "Directory src/CAMS.Tests not found."; exit 1; }

# Run tests with coverage
echo "Running tests with code coverage..."
dotnet test --collect:"XPlat Code Coverage"
if [ $? -ne 0 ]; then
    echo "Tests failed."
    exit 1
fi

# Go to TestResults
echo "Changing directory to TestResults..."
cd TestResults || { echo "TestResults directory not found."; exit 1; }

# Find the newest directory (GUID-like) by modification time
GUID_DIR=$(ls -dt1 ./*/ | head -n 1)
if [ -z "$GUID_DIR" ]; then
    echo "No GUID folder found in TestResults."
    exit 1
fi

echo "Newest GUID folder found: $GUID_DIR"
cd "$GUID_DIR" || { echo "Cannot access $GUID_DIR."; exit 1; }

# Generate coverage report
echo "Generating coverage report..."
reportgenerator "-reports:coverage.cobertura.xml" "-targetdir:coveragereport"

if [ ! -f "coveragereport/index.html" ]; then
    echo "Report generation failed or index.html not found."
    exit 1
fi

echo "Coverage report generated in folder 'coveragereport'."

# Try to open index.html automatically
echo "Attempting to open index.html..."

# Detect OS (Linux or macOS) for opening the file
unameOut="$(uname -s)"
case "${unameOut}" in
    Linux*)   xdg-open coveragereport/index.html >/dev/null 2>&1 ;;
    Darwin*)  open coveragereport/index.html ;;
    *)        echo "Open the file manually: coveragereport/index.html" ;;
esac
