#!/bin/bash
# ===================================================
# Docker Hub Push Script for StudioFlow
# ===================================================
# This script builds and pushes the Docker image to Docker Hub
# Usage: bash push-to-docker-hub.sh
# Or for specific tag: bash push-to-docker-hub.sh v1.0
# ===================================================

set -e  # Exit on any error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo "╔════════════════════════════════════════════════════════════╗"
echo "║     StudioFlow Docker Hub Push Script                      ║"
echo "╚════════════════════════════════════════════════════════════╝"
echo ""

# Default tag
TAG="${1:-v1.0}"
DOCKER_USERNAME="${DOCKER_HUB_USERNAME:-}"

# ===================================================
# STEP 1: Get Docker Hub Username
# ===================================================
echo -e "${YELLOW}[1/6] Getting Docker Hub Configuration...${NC}"

if [ -z "$DOCKER_USERNAME" ]; then
    read -p "Enter your Docker Hub username: " DOCKER_USERNAME
fi

if [ -z "$DOCKER_USERNAME" ]; then
    echo -e "${RED}❌ Docker Hub username is required${NC}"
    exit 1
fi

echo -e "${GREEN}✅ Using Docker Hub username: ${BLUE}$DOCKER_USERNAME${NC}"

# ===================================================
# STEP 2: Verify Docker is Running
# ===================================================
echo ""
echo -e "${YELLOW}[2/6] Verifying Docker Setup...${NC}"

if ! docker ps > /dev/null 2>&1; then
    echo -e "${RED}❌ Docker is not running or not accessible${NC}"
    echo "Please start Docker Desktop and try again"
    exit 1
fi

echo -e "${GREEN}✅ Docker is running${NC}"

# ===================================================
# STEP 3: Check for Dockerfile
# ===================================================
echo ""
echo -e "${YELLOW}[3/6] Checking Dockerfile...${NC}"

if [ ! -f "Dockerfile" ]; then
    echo -e "${RED}❌ Dockerfile not found in current directory${NC}"
    echo "Please run this script from the project root directory"
    exit 1
fi

echo -e "${GREEN}✅ Dockerfile found${NC}"

# ===================================================
# STEP 4: Build Docker Image
# ===================================================
echo ""
echo -e "${YELLOW}[4/6] Building Docker Image (this may take 2-5 minutes)...${NC}"
echo ""

IMAGE_NAME="studioflow"
BUILD_TAG="${DOCKER_USERNAME}/${IMAGE_NAME}:${TAG}"

echo -e "${BLUE}Building: $BUILD_TAG${NC}"
echo ""

docker build -t "$IMAGE_NAME:latest" -t "$IMAGE_NAME:$TAG" -t "$BUILD_TAG" .

if [ $? -eq 0 ]; then
    echo ""
    echo -e "${GREEN}✅ Docker image built successfully${NC}"
else
    echo ""
    echo -e "${RED}❌ Docker build failed${NC}"
    exit 1
fi

# ===================================================
# STEP 5: Login to Docker Hub
# ===================================================
echo ""
echo -e "${YELLOW}[5/6] Authenticating with Docker Hub...${NC}"

# Check if already logged in
if docker info | grep -q "Username: $DOCKER_USERNAME"; then
    echo -e "${GREEN}✅ Already logged in as $DOCKER_USERNAME${NC}"
else
    echo "Please login to Docker Hub when prompted"
    echo "(Enter your Docker Hub token or password)"
    echo ""
    
    if docker login -u "$DOCKER_USERNAME"; then
        echo -e "${GREEN}✅ Successfully logged in to Docker Hub${NC}"
    else
        echo -e "${RED}❌ Docker Hub login failed${NC}"
        exit 1
    fi
fi

# ===================================================
# STEP 6: Push Image to Docker Hub
# ===================================================
echo ""
echo -e "${YELLOW}[6/6] Pushing Image to Docker Hub...${NC}"
echo ""

echo -e "${BLUE}Pushing: $BUILD_TAG${NC}"
docker push "$BUILD_TAG"

if [ $? -eq 0 ]; then
    echo ""
    echo -e "${GREEN}✅ Image pushed successfully${NC}"
else
    echo ""
    echo -e "${RED}❌ Push failed${NC}"
    exit 1
fi

# Push latest tag
echo ""
echo -e "${BLUE}Also pushing: ${DOCKER_USERNAME}/${IMAGE_NAME}:latest${NC}"
docker tag "$IMAGE_NAME:latest" "${DOCKER_USERNAME}/${IMAGE_NAME}:latest"
docker push "${DOCKER_USERNAME}/${IMAGE_NAME}:latest"

# ===================================================
# STEP 7: Display Summary
# ===================================================
echo ""
echo -e "${GREEN}╔════════════════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║          🎉 Push Successful!                              ║${NC}"
echo -e "${GREEN}╚════════════════════════════════════════════════════════════╝${NC}"

echo ""
echo "Image Details:"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo -e "${BLUE}Repository:${NC} ${DOCKER_USERNAME}/studioflow"
echo -e "${BLUE}Tags:${NC} $TAG, latest"
echo -e "${BLUE}Docker Hub URL:${NC} https://hub.docker.com/r/${DOCKER_USERNAME}/studioflow"
echo ""

echo "Next Steps:"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "1. Verify on Docker Hub:"
echo -e "   ${YELLOW}https://hub.docker.com/r/${DOCKER_USERNAME}/studioflow${NC}"
echo ""
echo "2. Pull on EC2 instance:"
echo -e "   ${YELLOW}docker pull ${DOCKER_USERNAME}/studioflow:${TAG}${NC}"
echo ""
echo "3. Check local images:"
echo -e "   ${YELLOW}docker images | grep studioflow${NC}"
echo ""

echo "View Image Details:"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
docker images | grep studioflow

echo ""
echo -e "${GREEN}✅ Ready for EC2 Deployment!${NC}"
echo ""

