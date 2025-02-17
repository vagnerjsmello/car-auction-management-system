@echo off
REM This script installs reportgenerator if not present,
REM runs tests with coverage, finds the newest folder in TestResults,
REM generates an HTML report, and opens index.html in Windows.

echo Checking if reportgenerator is installed...
where reportgenerator >nul 2>&1
if errorlevel 1 (
    echo reportgenerator not found, installing...
    dotnet tool install --global dotnet-reportgenerator-globaltool
) else (
    echo reportgenerator is already installed.
)

echo Changing directory to src\CAMS.Tests...
cd src\CAMS.Tests || (echo Directory src\CAMS.Tests not found & exit /b 1)

echo Running tests with code coverage...
dotnet test --collect:"XPlat Code Coverage"
if errorlevel 1 (
    echo Tests failed.
    exit /b 1
)

echo Changing directory to TestResults...
cd TestResults || (echo TestResults folder not found & exit /b 1)

REM Find the newest folder by date/time
REM /o-d = order by date descending, /b = bare, /ad = only directories
set "NEWEST="
for /f "delims=" %%i in ('dir /b /ad /o-d') do (
    if not defined NEWEST set NEWEST=%%i
)

if "%NEWEST%"=="" (
    echo No GUID folder found in TestResults.
    exit /b 1
)

echo Newest GUID folder: %NEWEST%
cd "%NEWEST%"

echo Generating coverage report...
reportgenerator "-reports:coverage.cobertura.xml" "-targetdir:coveragereport"

if not exist "coveragereport\index.html" (
    echo Report generation failed or index.html not found.
    exit /b 1
)

echo Coverage report generated in folder "coveragereport".
echo Opening index.html...
start coveragereport\index.html

pause
