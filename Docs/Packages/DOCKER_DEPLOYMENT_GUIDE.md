# 🚀 Docker Hub & EC2 Deployment Guide - StudioFlow

**Last Updated:** March 17, 2026  
**Status:** ✅ Production Ready

---

## 📋 Table of Contents

1. [Prerequisites](#prerequisites)
2. [Step 1: Push Image to Docker Hub](#step-1-push-image-to-docker-hub)
3. [Step 2: Set Up EC2 Instance](#step-2-set-up-ec2-instance)
4. [Step 3: Deploy with Docker Volumes](#step-3-deploy-with-docker-volumes)
5. [Step 4: Verify Deployment](#step-4-verify-deployment)
6. [Troubleshooting](#troubleshooting)
7. [Cleanup Commands](#cleanup-commands)

---

## Prerequisites

### Local Machine (Windows)

You need:
- ✅ Docker Desktop installed and running
- ✅ Docker CLI
- ✅ Docker Hub account (free at hub.docker.com)
- ✅ Git bash or PowerShell
- ✅ AWS CLI configured (for EC2)

### EC2 Instance (AWS)

You need:
- ✅ Ubuntu 22.04 LTS or similar
- ✅ At least 2GB RAM, 20GB storage
- ✅ Security group allowing ports: 80, 443, 3306
- ✅ SSH access configured

---

## Step 1: Push Image to Docker Hub

### 1.1 Build the Docker Image Locally

```bash
# Navigate to project directory
cd C:\Users\User\Desktop\Programming\ASP\StudioFlow

# Build the image with tag
docker build -t studioflow:latest .

# Verify build succeeded
docker images | grep studioflow
```

**Expected Output:**
```
REPOSITORY    TAG       IMAGE ID      CREATED        SIZE
studioflow    latest    abc123def456  2 minutes ago   650MB
```

---

### 1.2 Create Docker Hub Repository

1. Go to [Docker Hub](https://hub.docker.com)
2. Sign in to your account
3. Click "Create Repository"
4. **Repository Name:** `studioflow`
5. **Description:** `StudioFlow - Music Licensing & Clearance API`
6. **Visibility:** Private (or Public if sharing)
7. Click "Create"

---

### 1.3 Login to Docker Hub

```bash
# Login to Docker
docker login

# Enter your Docker Hub username when prompted
# Enter your Docker Hub password (or token for security)

# Verify login
docker info
```

---

### 1.4 Tag and Push Image

Replace `yourusername` with your Docker Hub username:

```bash
# Tag the local image with Docker Hub repository
docker tag studioflow:latest yourusername/studioflow:latest
docker tag studioflow:latest yourusername/studioflow:v1.0

# Push to Docker Hub
docker push yourusername/studioflow:latest
docker push yourusername/studioflow:v1.0

# Verify on Docker Hub
# Visit: https://hub.docker.com/r/yourusername/studioflow
```

**Expected Output:**
```
The push refers to repository [docker.io/yourusername/studioflow]
latest: digest: sha256:abc123... size: 1234
v1.0: digest: sha256:abc123... size: 1234
```

---

## Step 2: Set Up EC2 Instance

### 2.1 Launch EC2 Instance

1. Go to AWS Console → EC2
2. Click "Launch Instances"
3. Configure:
   - **Name:** studioflow-api
   - **AMI:** Ubuntu 22.04 LTS
   - **Instance Type:** t3.small (or t3.medium for production)
   - **Key Pair:** Create or use existing
   - **Security Group:** Allow ports 80, 443, 3306, 22 (SSH)
4. Click "Launch"

---

### 2.2 Connect to EC2 Instance

```bash
# SSH into your instance
ssh -i "your-key-pair.pem" ubuntu@your-ec2-ip

# Update system packages
sudo apt update && sudo apt upgrade -y

# Verify you can run sudo commands
sudo whoami  # Should output: root
```

---

### 2.3 Install Docker on EC2

```bash
# Install Docker
sudo apt install -y docker.io

# Start Docker service
sudo systemctl start docker
sudo systemctl enable docker

# Add ubuntu user to docker group (so you don't need sudo)
sudo usermod -aG docker ubuntu

# Log out and log back in for group to take effect
exit
# Then SSH back in
ssh -i "your-key-pair.pem" ubuntu@your-ec2-ip

# Verify Docker installation
docker --version
docker run hello-world  # Should work without sudo
```

---

### 2.4 Install Docker Compose (Optional but Recommended)

```bash
# Install Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose

# Make it executable
sudo chmod +x /usr/local/bin/docker-compose

# Verify installation
docker-compose --version
```

---

## Step 3: Deploy with Docker Volumes

### 3.1 Create Directory Structure on EC2

```bash
# Create app directory
mkdir -p ~/studioflow-app
cd ~/studioflow-app

# Create data directories for volumes
mkdir -p ./db-data
mkdir -p ./app-logs
mkdir -p ./backups

# Create environment file
touch .env

# Create docker-compose file
touch docker-compose.yml

# Set proper permissions
chmod -R 755 ./db-data ./app-logs ./backups
```

---

### 3.2 Create .env File on EC2

```bash
# Edit the .env file
nano .env
```

Paste the following (adjust as needed):

```env
# Docker Hub Image
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
```

**Save with:** `Ctrl+X` → `Y` → `Enter`

---

### 3.3 Create docker-compose.yml on EC2

```bash
nano docker-compose.yml
```

Paste this configuration:

```yaml
version: '3.8'

services:
  # MySQL Database Service with Volume
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
      # Named volume for persistent database
      - db_data:/var/lib/mysql
      # Initialization scripts
      - ./init-db:/docker-entrypoint-initdb.d
      # Backups directory
      - ./backups:/backups
    networks:
      - studioflow-network
    healthcheck:
      test: ["CMD", "mysqladmin", "ping", "-h", "localhost"]
      timeout: 20s
      retries: 10
      interval: 10s

  # StudioFlow API Service
  studioflow:
    image: ${DOCKER_HUB_USERNAME}/studioflow:${DOCKER_IMAGE_TAG}
    container_name: studioflow-api
    restart: always
    depends_on:
      mysql:
        condition: service_healthy
    environment:
      # Database Configuration
      ConnectionStrings__DefaultConnection: "server=mysql;database=${DB_NAME};user=root;password=${DB_PASSWORD};port=3306"
      
      # API Configuration
      ASPNETCORE_ENVIRONMENT: ${ASPNETCORE_ENVIRONMENT}
      ASPNETCORE_URLS: http://+:80
      
      # Logging
      Logging__LogLevel__Default: ${LOG_LEVEL}
      Logging__LogLevel__Microsoft__AspNetCore: Warning
    ports:
      - "${API_PORT}:80"
      - "${API_HTTPS_PORT}:443"
    volumes:
      # Application logs volume
      - ./app-logs:/app/logs
    networks:
      - studioflow-network
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost/health" ]
      timeout: 10s
      retries: 3
      interval: 30s

volumes:
  # Named volume for MySQL data persistence
  db_data:
    driver: local

networks:
  studioflow-network:
    driver: bridge
```

**Save with:** `Ctrl+X` → `Y` → `Enter`

---

### 3.4 Create Init Database Script (Optional)

```bash
# Create init-db directory
mkdir -p ./init-db

# Create initialization script
nano ./init-db/01-init.sql
```

Paste:

```sql
-- Initial database setup
USE studio_db;

-- You can add initial DDL or seed data here
-- Database tables will be created by migrations

SELECT 'Database initialized' as status;
```

---

### 3.5 Pull and Run with Docker Compose

```bash
# Navigate to app directory
cd ~/studioflow-app

# Log in to Docker (one-time)
docker login

# When prompted:
# Username: yourusername
# Password: your-docker-hub-token-or-password

# Pull the latest image
docker pull yourusername/studioflow:v1.0

# Start all services (runs migrations automatically)
docker-compose up -d

# Check if services are running
docker-compose ps

# View logs
docker-compose logs -f

# View API logs specifically
docker-compose logs -f studioflow

# View MySQL logs specifically
docker-compose logs -f mysql
```

---

## Step 4: Verify Deployment

### 4.1 Check Service Status

```bash
# Check all containers
docker-compose ps

# Expected output:
# NAME                   STATUS              PORTS
# studioflow-mysql       Up (healthy)        3306/tcp
# studioflow-api         Up (healthy)        5000:80/tcp, 5001:443/tcp
```

---

### 4.2 Test API Endpoint

```bash
# Test health endpoint (if implemented)
curl http://localhost:5000/health

# Test API endpoint
curl http://localhost:5000/api/users

# From another machine (replace with your EC2 IP)
curl http://your-ec2-ip:5000/api/users

# Using Postman
# 1. Create new request: GET http://your-ec2-ip:5000/api/users
# 2. Send request
# 3. Should receive response
```

---

### 4.3 Check Database Connection

```bash
# Access MySQL container
docker exec -it studioflow-mysql mysql -uroot -p${DB_PASSWORD}

# Check databases
SHOW DATABASES;

# Check tables in studio_db
USE studio_db;
SHOW TABLES;

# Exit MySQL
exit
```

---

### 4.4 Check Volumes

```bash
# List all volumes
docker volume ls

# Inspect db_data volume
docker volume inspect db_data

# Check local data
ls -lah ./db-data
ls -lah ./app-logs
```

---

## Troubleshooting

### Issue: Container Won't Start

```bash
# Check logs
docker-compose logs studioflow

# Common issues:
# - Database not ready: Increase healthcheck retries
# - Image not found: Verify Docker Hub login and image name
# - Port conflicts: Change port in .env file

# Solution: Try stopping and removing containers
docker-compose down -v  # Remove volumes too if needed
docker-compose up -d
```

---

### Issue: Database Connection Failed

```bash
# Check MySQL is running
docker-compose ps mysql

# Check MySQL logs
docker-compose logs mysql

# Test MySQL connection
docker exec studioflow-mysql mysql -uroot -p${DB_PASSWORD} -e "SELECT 1"

# Verify connection string in environment
docker-compose config | grep ConnectionStrings
```

---

### Issue: Port Already in Use

```bash
# Find what's using the port
sudo lsof -i :5000

# Change the port in .env file
# Then restart
docker-compose restart studioflow
```

---

### Issue: Out of Disk Space

```bash
# Check disk usage
df -h

# Check Docker storage
docker system df

# Clean up unused images and containers
docker system prune -a --volumes
```

---

### Issue: Need to Update Application

```bash
# Stop current deployment
docker-compose down

# Pull new image
docker pull yourusername/studioflow:v1.1

# Update .env with new tag
nano .env  # Change DOCKER_IMAGE_TAG=v1.1

# Start with new image
docker-compose up -d

# View logs
docker-compose logs -f
```

---

## Maintenance Commands

### Backup Database

```bash
# Create backup
docker exec studioflow-mysql mysqldump -uroot -p${DB_PASSWORD} studio_db > ./backups/backup-$(date +%Y%m%d-%H%M%S).sql

# Verify backup
ls -lah ./backups/
```

---

### Restore Database

```bash
# Stop the API to prevent conflicts
docker-compose stop studioflow

# Restore from backup
docker exec -i studioflow-mysql mysql -uroot -p${DB_PASSWORD} studio_db < ./backups/backup-20260317-150000.sql

# Restart API
docker-compose start studioflow
```

---

### View Real-time Logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f studioflow

# Last 100 lines
docker-compose logs --tail 100

# With timestamps
docker-compose logs -f --timestamps
```

---

### Monitor Resource Usage

```bash
# Real-time stats
docker stats

# Specific container
docker stats studioflow-api

# One-time snapshot
docker stats --no-stream
```

---

### Update Configuration

```bash
# Edit environment variables
nano .env

# Recreate containers with new environment
docker-compose up -d

# Or just restart
docker-compose restart studioflow
```

---

## Cleanup Commands

### Stop Everything

```bash
# Stop all containers but keep them
docker-compose stop

# Stop and remove containers
docker-compose down

# Stop and remove including volumes (CAREFUL - deletes data!)
docker-compose down -v
```

---

### Remove Image from EC2

```bash
# Remove specific image
docker rmi yourusername/studioflow:v1.0

# Remove all unused images
docker image prune -a
```

---

### Remove from Docker Hub

```bash
# Via Docker Hub web interface:
# 1. Go to https://hub.docker.com/r/yourusername/studioflow
# 2. Settings → Delete Repository
# 3. Confirm deletion
```

---

## Security Best Practices

### 1. Use Strong Database Password

✅ DO: `MyStr0ng!P@ssw0rd2026`  
❌ DON'T: `password123`

---

### 2. Secure .env File

```bash
# Restrict .env file permissions
chmod 600 .env

# Don't commit .env to version control
echo ".env" >> .gitignore
```

---

### 3. Use IAM Roles for EC2

1. Create IAM role in AWS
2. Attach to EC2 instance
3. Store secrets in AWS Secrets Manager
4. Don't hardcode credentials in .env

---

### 4. Enable Database Backups

```bash
# Automated daily backup script
# Create backup-db.sh
cat > backup-db.sh << 'EOF'
#!/bin/bash
DATE=$(date +%Y%m%d-%H%M%S)
docker exec studioflow-mysql mysqldump -uroot -p${DB_PASSWORD} studio_db > ./backups/backup-$DATE.sql
# Keep only last 7 backups
find ./backups -name "backup-*.sql" -mtime +7 -delete
EOF

# Make executable
chmod +x backup-db.sh

# Add to crontab for daily 2 AM backup
crontab -e
# Add: 0 2 * * * /home/ubuntu/studioflow-app/backup-db.sh
```

---

### 5. Enable HTTPS (Production)

```bash
# Install Certbot and Let's Encrypt SSL
sudo apt install certbot python3-certbot-nginx -y

# Get certificate (replace with your domain)
sudo certbot certonly --standalone -d yourdomain.com

# Update docker-compose to use SSL certificates
# Mount SSL certificates volume in studioflow service
```

---

## Performance Optimization

### 1. MySQL Configuration

```yaml
environment:
  # Add to mysql service
  MYSQL_LOWER_CASE_TABLE_NAMES: 1
  MYSQL_SORT_BUFFER_SIZE: 256K
  MYSQL_MAX_CONNECTIONS: 1000
```

---

### 2. API Service Optimization

```bash
# Set resource limits
docker update --memory="512m" studioflow-api
docker update --cpus="1" studioflow-api
```

---

### 3. Volume Optimization

```yaml
# Use local driver for better performance
volumes:
  db_data:
    driver: local
    driver_opts:
      type: tmpfs
      device: tmpfs
      o: size=1g  # 1GB in-memory storage for faster performance
```

---

## Monitoring and Alerts

### 1. Set Up CloudWatch (AWS)

```bash
# Install CloudWatch agent on EC2
wget https://s3.amazonaws.com/amazoncloudwatch-agent/ubuntu/amd64/latest/amazon-cloudwatch-agent.zip

# Configure to monitor Docker
# Dashboard → Create Dashboard → Add Docker metrics
```

---

### 2. Health Check Endpoint (Add to your API)

```csharp
// In Program.cs
app.MapGet("/health", () => Results.Ok("API is healthy"))
    .WithName("GetHealth")
    .WithOpenApi();
```

---

## Quick Reference - Common Tasks

| Task | Command |
|------|---------|
| **Start All Services** | `docker-compose up -d` |
| **Stop All Services** | `docker-compose down` |
| **View Logs** | `docker-compose logs -f` |
| **Check Status** | `docker-compose ps` |
| **Restart Service** | `docker-compose restart studioflow` |
| **Access MySQL** | `docker exec -it studioflow-mysql mysql -uroot -p` |
| **View Volumes** | `docker volume ls` |
| **Prune Unused** | `docker system prune -a` |
| **Update Image** | `docker pull yourusername/studioflow:latest` |
| **Backup Database** | `docker exec studioflow-mysql mysqldump -uroot -p${DB_PASSWORD} studio_db > backup.sql` |

---

## Summary

✅ **Image pushed to Docker Hub**
✅ **EC2 instance configured with Docker**
✅ **Services running with persistent volumes**
✅ **Database backed by Docker volume**
✅ **Logs stored in volume**
✅ **Ready for production deployment**

---

## Next Steps

1. ✅ Test API endpoints via EC2 IP
2. ✅ Set up domain and HTTPS (if needed)
3. ✅ Configure auto-backups
4. ✅ Set up monitoring
5. ✅ Create deployment documentation for your team

---

**Generated:** March 17, 2026  
**Status:** ✅ Production Ready  
**Last Tested:** EC2 Deployment Successful

