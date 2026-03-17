param(
    [string]$Tag = "v1.0",
    [string]$DockerUsername = $env:DOCKER_HUB_USERNAME
)

# ===================================================
# Docker Hub Push Script for StudioFlow (PowerShell)
# ===================================================
# Usage: .\push-to-docker-hub.ps1
# Or: .\push-to-docker-hub.ps1 -Tag v1.0 -DockerUsername yourusername
# ===================================================

$ErrorActionPreference = "Stop"

# Colors
function Write-Success { Write-Host $args -ForegroundColor Green }
function Write-Error { Write-Host $args -ForegroundColor Red }
function Write-Warning { Write-Host $args -ForegroundColor Yellow }
function Write-Info { Write-Host $args -ForegroundColor Cyan }

Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║     StudioFlow Docker Hub Push Script (PowerShell)        ║" -ForegroundColor Green
Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""

# ===================================================
# STEP 1: Get Docker Hub Username
# ===================================================
Write-Warning "[1/6] Getting Docker Hub Configuration..."

if ([string]::IsNullOrWhiteSpace($DockerUsername)) {
    $DockerUsername = Read-Host "Enter your Docker Hub username"
}

if ([string]::IsNullOrWhiteSpace($DockerUsername)) {
    Write-Error "Docker Hub username is required"
    exit 1
}

Write-Success "Using Docker Hub username: $DockerUsername"

# ===================================================
# STEP 2: Verify Docker is Running
# ===================================================
Write-Host ""
Write-Warning "[2/6] Verifying Docker Setup..."

try {
    $output = docker ps 2>&1
    Write-Success "Docker is running"
}
catch {
    Write-Error "Docker is not running or not accessible"
    Write-Host "Please start Docker Desktop and try again"
    exit 1
}

# ===================================================
# STEP 3: Check for Dockerfile
# ===================================================
Write-Host ""
Write-Warning "[3/6] Checking Dockerfile..."

if (!(Test-Path "Dockerfile")) {
    Write-Error "Dockerfile not found in current directory"
    Write-Host "Please run this script from the project root directory"
    exit 1
}

Write-Success "Dockerfile found"

# ===================================================
# STEP 4: Build Docker Image
# ===================================================
Write-Host ""
Write-Warning "[4/6] Building Docker Image (this may take 2-5 minutes)..."
Write-Host ""

$ImageName = "studioflow"
$BuildTag = "$DockerUsername/$ImageName:$Tag"

Write-Info "Building: $BuildTag"
Write-Host ""

try {
    docker build -t "$ImageName`:latest" -t "$ImageName`:$Tag" -t "$BuildTag" .
    Write-Success "Docker image built successfully"
}
catch {
    Write-Error "Docker build failed: $_"
    exit 1
}

# ===================================================
# STEP 5: Login to Docker Hub
# ===================================================
Write-Host ""
Write-Warning "[5/6] Authenticating with Docker Hub..."

try {
    $currentUser = docker info 2>&1 | Select-String "Username"
    if ($currentUser -match $DockerUsername) {
        Write-Success "Already logged in as $DockerUsername"
    } else {
        Write-Host "Please login to Docker Hub when prompted"
        Write-Host "(Enter your Docker Hub token or password)"
        Write-Host ""
        
        docker login -u $DockerUsername
        Write-Success "Successfully logged in to Docker Hub"
    }
}
catch {
    Write-Error "Docker Hub login failed: $_"
    exit 1
}

# ===================================================
# STEP 6: Push Image to Docker Hub
# ===================================================
Write-Host ""
Write-Warning "[6/6] Pushing Image to Docker Hub..."
Write-Host ""

Write-Info "Pushing: $BuildTag"

try {
    docker push $BuildTag
    Write-Success "Image pushed successfully"
}
catch {
    Write-Error "Push failed: $_"
    exit 1
}

# Push latest tag
Write-Host ""
Write-Info "Also pushing: $DockerUsername/$ImageName`:latest"

try {
    docker tag "$ImageName`:latest" "$DockerUsername/$ImageName`:latest"
    docker push "$DockerUsername/$ImageName`:latest"
    Write-Success "Latest tag pushed"
}
catch {
    Write-Error "Failed to push latest tag: $_"
    exit 1
}

# ===================================================
# STEP 7: Display Summary
# ===================================================
Write-Host ""
Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║          🎉 Push Successful!                              ║" -ForegroundColor Green
Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Green

Write-Host ""
Write-Host "Image Details:" -ForegroundColor Yellow
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
Write-Info "Repository: $DockerUsername/studioflow"
Write-Info "Tags: $Tag, latest"
Write-Info "Docker Hub URL: https://hub.docker.com/r/$DockerUsername/studioflow"
Write-Host ""

Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
Write-Host ""
Write-Host "1. Verify on Docker Hub:"
Write-Warning "   https://hub.docker.com/r/$DockerUsername/studioflow"
Write-Host ""
Write-Host "2. Pull on EC2 instance:"
Write-Warning "   docker pull $DockerUsername/studioflow:$Tag"
Write-Host ""
Write-Host "3. Check local images:"
Write-Warning "   docker images | findstr studioflow"
Write-Host ""

Write-Host "View Image Details:" -ForegroundColor Yellow
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
docker images | Select-String studioflow

Write-Host ""
Write-Success "✅ Ready for EC2 Deployment!"
Write-Host ""

