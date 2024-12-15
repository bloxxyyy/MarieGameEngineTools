# Ensure the script stops on errors
$ErrorActionPreference = "Stop"

# Get the directory of the script
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# Path to the migration manager
$TestFolder = Join-Path $ScriptDir "MarieBaseMigrationManager"

# Navigate to the test folder
Push-Location $TestFolder

try {
    Write-Host "Building the C# project..."
    dotnet build
    Write-Host "Running the project with the 'migrate' argument..."
    dotnet run -- migrate
}
catch {
    Write-Error "An error occurred: $_"
}
finally {
    # Return to the original location
    Pop-Location
}