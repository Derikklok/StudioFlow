#!/bin/bash
# ===================================================
# StudioFlow EC2 Setup Script
# ===================================================
# This script automates the setup of StudioFlow on EC2
# Usage: bash setup-ec2.sh
# ===================================================

set -e  # Exit on any error

echo "╔════════════════════════════════════════════════════════════╗"
echo "║     StudioFlow EC2 Setup & Deployment Script              ║"
echo "╚════════════════════════════════════════════════════════════╝"
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# ===================================================
# STEP 1: Verify Prerequisites
# ===================================================
echo -e "${YELLOW}[1/6] Verifying Prerequisites...${NC}"

if ! command -v docker &> /dev/null; then
    echo -e "${RED}❌ Docker is not installed${NC}"
    exit 1
fi
echo -e "${GREEN}✅ Docker is installed${NC}"

if ! command -v docker-compose &> /dev/null; then
    echo -e "${YELLOW}⚠️  Docker Compose not found. Installing...${NC}"
    sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
    sudo chmod +x /usr/local/bin/docker-compose
    echo -e "${GREEN}✅ Docker Compose installed${NC}"
else
    echo -e "${GREEN}✅ Docker Compose is installed${NC}"
fi

# ===================================================
# STEP 2: Create Directory Structure
# ===================================================
echo ""
echo -e "${YELLOW}[2/6] Creating Directory Structure...${NC}"

APP_DIR="$HOME/studioflow-app"

if [ -d "$APP_DIR" ]; then
    echo -e "${YELLOW}⚠️  Directory $APP_DIR already exists${NC}"
else
    mkdir -p "$APP_DIR"
    echo -e "${GREEN}✅ Created $APP_DIR${NC}"
fi

cd "$APP_DIR"

# Create subdirectories
for dir in db-data app-logs backups init-db certs; do
    if [ ! -d "./$dir" ]; then
        mkdir -p "./$dir"
        echo -e "${GREEN}✅ Created $dir directory${NC}"
    fi
done

# Set permissions
chmod -R 755 ./db-data ./app-logs ./backups ./init-db

# ===================================================
# STEP 3: Create Environment File
# ===================================================
echo ""
echo -e "${YELLOW}[3/6] Setting up Configuration Files...${NC}"

if [ -f "./.env" ]; then
    echo -e "${YELLOW}⚠️  .env file already exists. Skipping...${NC}"
else
    cat > .env << 'EOF'
# Docker Hub Configuration
DOCKER_HUB_USERNAME=yourusername
DOCKER_IMAGE_TAG=v1.0

# Database Configuration
DB_HOST=mysql
DB_PORT=3306
DB_NAME=studio_db
DB_USER=root
DB_PASSWORD=YourStrongPassword123!

# API Configuration
API_PORT=5000
API_HTTPS_PORT=5001
ASPNETCORE_ENVIRONMENT=Production

# Logging
LOG_LEVEL=Information
EOF
    echo -e "${GREEN}✅ Created .env file${NC}"
    echo -e "${YELLOW}⚠️  IMPORTANT: Edit .env with your Docker Hub username and strong password!${NC}"
    echo "   Run: nano .env"
fi

chmod 600 .env

# ===================================================
# STEP 4: Create docker-compose File
# ===================================================
echo ""
echo -e "${YELLOW}[4/6] Creating Docker Compose Configuration...${NC}"

cat > docker-compose.yml << 'EOF'
version: '3.8'

services:
  mysql:
    image: mysql:8.0
    container_name: studioflow-mysql
    restart: always
    environment:
      MYSQL_ROOT_PASSWORD: ${DB_PASSWORD}
      MYSQL_DATABASE: ${DB_NAME}
      MYSQL_USER: root
      MYSQL_PASSWORD: ${DB_PASSWORD}
      MYSQL_PORT: 3306
    ports:
      - "${DB_PORT}:3306"
    volumes:
      - db_data:/var/lib/mysql
      - ./init-db:/docker-entrypoint-initdb.d
      - ./backups:/backups
    networks:
      - studioflow-network
    healthcheck:
      test: ["CMD", "mysqladmin", "ping", "-h", "localhost"]
      timeout: 20s
      retries: 10
      interval: 10s

  studioflow:
    image: ${DOCKER_HUB_USERNAME}/studioflow:${DOCKER_IMAGE_TAG}
    container_name: studioflow-api
    restart: always
    depends_on:
      mysql:
        condition: service_healthy
    environment:
      ConnectionStrings__DefaultConnection: "server=mysql;database=${DB_NAME};user=root;password=${DB_PASSWORD};port=3306"
      ASPNETCORE_ENVIRONMENT: ${ASPNETCORE_ENVIRONMENT}
      ASPNETCORE_URLS: http://+:80
      Logging__LogLevel__Default: ${LOG_LEVEL}
      Logging__LogLevel__Microsoft__AspNetCore: Warning
    ports:
      - "${API_PORT}:80"
      - "${API_HTTPS_PORT}:443"
    volumes:
      - ./app-logs:/app/logs
    networks:
      - studioflow-network
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost/health" ]
      timeout: 10s
      retries: 3
      interval: 30s

volumes:
  db_data:
    driver: local

networks:
  studioflow-network:
    driver: bridge
EOF
echo -e "${GREEN}✅ Created docker-compose.yml${NC}"

# ===================================================
# STEP 5: Docker Hub Login
# ===================================================
echo ""
echo -e "${YELLOW}[5/6] Docker Hub Authentication...${NC}"

read -p "Do you want to login to Docker Hub now? (y/n) " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    docker login
    echo -e "${GREEN}✅ Logged in to Docker Hub${NC}"
else
    echo -e "${YELLOW}⚠️  You can login later with: docker login${NC}"
fi

# ===================================================
# STEP 6: Ready for Deployment
# ===================================================
echo ""
echo -e "${YELLOW}[6/6] Setup Complete!${NC}"

echo ""
echo -e "${GREEN}╔════════════════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║          🎉 Setup Complete! Ready to Deploy              ║${NC}"
echo -e "${GREEN}╚════════════════════════════════════════════════════════════╝${NC}"

echo ""
echo "Next Steps:"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "1. Edit .env file with your Docker Hub credentials and strong password:"
echo -e "   ${YELLOW}nano .env${NC}"
echo ""
echo "2. Start the services:"
echo -e "   ${YELLOW}cd $APP_DIR${NC}"
echo -e "   ${YELLOW}docker-compose up -d${NC}"
echo ""
echo "3. Check service status:"
echo -e "   ${YELLOW}docker-compose ps${NC}"
echo ""
echo "4. View logs:"
echo -e "   ${YELLOW}docker-compose logs -f${NC}"
echo ""
echo "5. Test the API:"
echo -e "   ${YELLOW}curl http://localhost:5000/api/health${NC}"
echo ""
echo "Configuration Location: $APP_DIR"
echo ""

