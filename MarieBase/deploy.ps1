#param should be: "start" or "stop"
param (
    [string]$action
)

# Define the environment variables
$env:MONGO_PORT = "27017"
$env:MONGO_USER = "admin"
$env:MONGO_PASSWORD = "password"
$env:MONGO_HOST = "localhost"
$env:MONGO_CONTAINER_NAME = "mongodb"

# Permanently set the environment variables for the user account
[System.Environment]::SetEnvironmentVariable("MARIE_MONGO_PORT", $env:MONGO_PORT, [System.EnvironmentVariableTarget]::User)
[System.Environment]::SetEnvironmentVariable("MARIE_MONGO_USER", $env:MONGO_USER, [System.EnvironmentVariableTarget]::User)
[System.Environment]::SetEnvironmentVariable("MARIE_MONGO_PASSWORD", $env:MONGO_PASSWORD, [System.EnvironmentVariableTarget]::User)
[System.Environment]::SetEnvironmentVariable("MARIE_MONGO_HOST", $env:MONGO_HOST, [System.EnvironmentVariableTarget]::User)

# Display a message confirming the variables were set
Write-Host "Environment variables set:"
Write-Host "`tMARIE_MONGO_PORT: $env:MONGO_PORT"
Write-Host "`tMARIE_MONGO_USER: $env:MONGO_USER"
Write-Host "`tMARIE_MONGO_PASSWORD: $env:MONGO_PASSWORD"
Write-Host "`tMARIE_MONGO_HOST: $env:MONGO_HOST"


# Function to start Docker Compose and create the volume
function Start-DockerCompose {
    Write-Host "Starting MongoDB container and creating volume..."
    docker-compose up -d

    # Wait for the container to be ready
    Start-Sleep -Seconds 10

    $containerStatus = docker inspect -f "{{.State.Running}}" $env:MONGO_CONTAINER_NAME

    if ($containerStatus -eq "true") {
        Write-Host "MongoDB container is running. Executing migrate.ps1..."
        . ./migrate.ps1
    } else {
        Write-Error "Failed to start MongoDB container."
    }
}

# Function to stop Docker Compose and remove the volume
function Stop-DockerCompose {
    Write-Host "Stopping MongoDB container and removing volume..."
    docker-compose down -v
}

# Check if the action parameter is "start" or "stop"
if ($action -eq "start") {
    Start-DockerCompose
} elseif ($action -eq "stop") {
    Stop-DockerCompose
} else {
    Write-Host "Invalid action. Please use 'start' or 'stop'."
}