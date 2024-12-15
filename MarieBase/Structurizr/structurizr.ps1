#param should be: "generate" or "serve"
param (
    [string]$action
)

$env:PATH_TO_structurizr_BAT = "C:\MarieGameEngine\structurizr\structurizr-site-generatr-1.5.0\bin\structurizr-site-generatr.bat"
$workspaceFile = "workspace.dsl"
$assetsDir = "assets"

function generate {
    Write-Host "Generating structurizr project..."

    if (-Not (Test-Path $workspaceFile)) {
        Write-Host "Error: Workspace file '$workspaceFile' not found!" -ForegroundColor Red
        return
    }

    if (-Not (Test-Path $assetsDir)) {
        Write-Host "Error: Assets directory '$assetsDir' not found!" -ForegroundColor Red
        return
    }

    & $env:PATH_TO_structurizr_BAT generate-site --workspace-file $workspaceFile --assets-dir $assetsDir
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Site generation completed successfully!" -ForegroundColor Green
    } else {
        Write-Host "Error during site generation." -ForegroundColor Red
    }
}

function serve {
    Write-Host "Serving structurizr web instance..."
    & $env:PATH_TO_structurizr_BAT serve  --workspace-file $workspaceFile --assets-dir $assetsDir

    if ($LASTEXITCODE -eq 0) {
        Write-Host "Structurizr server started successfully!" -ForegroundColor Green
    } else {
        Write-Host "Error starting Structurizr server." -ForegroundColor Red
    }
}

if ($action -eq "generate") {
    generate
} elseif ($action -eq "serve") {
    serve
} else {
    Write-Host "Invalid action. Please use 'generate' or 'serve'."
}