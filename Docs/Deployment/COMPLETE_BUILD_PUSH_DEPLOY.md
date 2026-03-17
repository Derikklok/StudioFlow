# 🚀 Complete Build, Push & Deploy Guide

**Build Docker Image → Push to Docker Hub → Deploy to EC2**

Last Updated: March 17, 2026

---

## 📋 Table of Contents

1. [Part 1: Build Docker Image Locally](#part-1-build-docker-image-locally)
2. [Part 2: Push to Docker Hub](#part-2-push-to-docker-hub)
3. [Part 3: Set Up EC2 Instance](#part-3-set-up-ec2-instance)
4. [Part 4: Deploy to EC2](#part-4-deploy-to-ec2)
5. [Part 5: Verify Deployment](#part-5-verify-deployment)

---

## ✅ Prerequisites (Check These First)

### Local Machine (Windows)
- ✅ Docker Desktop installed and running
- ✅ Git Bash or PowerShell terminal
- ✅ Docker Hub account (free: https://hub.docker.com)

### AWS
- ✅ AWS Account with EC2 access
- ✅ EC2 instance (Amazon Linux 2 or Amazon Linux 2023 recommended)
- ✅ SSH key pair created and downloaded
- ✅ Security group allows ports: 22, 80, 443, 3306, 5000-5001

---

# PART 1: Build Docker Image Locally

## Step 1.1: Navigate to Project Directory

Open PowerShell or Git Bash:

```bash
cd C:\Users\User\Desktop\Programming\ASP\StudioFlow
```

Verify you're in the right directory:
```bash
ls  # Should show: docker-compose.yml, Dockerfile, appsettings.json, etc.
```

---

## Step 1.2: Verify Docker is Running

```bash
docker --version
# Expected output: Docker version 20.x.x or higher

docker ps
# Should show list of running containers (even if empty)
```

If Docker isn't running, open Docker Desktop application.

---

## Step 1.3: Build the Image

```bash
docker build -t studioflow:latest .
```

**What this does:**
- Reads the `Dockerfile`
- Builds the application
- Creates a local Docker image named `studioflow` with tag `latest`

**Expected output:**
```
[+] Building 45.3s (18/18)
...
=> => naming to docker.io/library/studioflow:latest
```

---

## Step 1.4: Verify Local Image Built Successfully

```bash
docker images | grep studioflow
```

**Expected output:**
```
REPOSITORY    TAG       IMAGE ID       CREATED         SIZE
studioflow    latest    abc123def456   2 minutes ago    650MB
```

✅ **Local image build complete!**

---

# PART 2: Push to Docker Hub

## Step 2.1: Create Docker Hub Repository

1. Go to: https://hub.docker.com
2. Sign in to your account (or create one if needed)
3. Click **"Create Repository"**
4. Fill in:
   - **Repository Name:** `studioflow`
   - **Description:** `StudioFlow - Music Licensing & Clearance API`
   - **Visibility:** `Private` (or Public if you want to share)
5. Click **"Create"**

Note your Docker Hub username (you'll need it in next steps).

---

## Step 2.2: Login to Docker from Your Computer

```bash
docker login
```

When prompted:
- **Username:** Enter your Docker Hub username
- **Password:** Enter your Docker Hub password (or personal access token for better security)

**Expected output:**
```
Login Succeeded
```

---

## Step 2.3: Tag the Image for Docker Hub

Replace `YOUR_DOCKER_HUB_USERNAME` with your actual Docker Hub username:

```bash
docker tag studioflow:latest YOUR_DOCKER_HUB_USERNAME/studioflow:latest
docker tag studioflow:latest YOUR_DOCKER_HUB_USERNAME/studioflow:v1.0
```

**Example** (if your username is "john123"):
```bash
docker tag studioflow:latest john123/studioflow:latest
docker tag studioflow:latest john123/studioflow:v1.0
```

---

## Step 2.4: Push to Docker Hub

```bash
docker push YOUR_DOCKER_HUB_USERNAME/studioflow:latest
docker push YOUR_DOCKER_HUB_USERNAME/studioflow:v1.0
```

**Expected output:**
```
The push refers to repository [docker.io/YOUR_DOCKER_HUB_USERNAME/studioflow]
latest: digest: sha256:abc123... size: 1234567
v1.0: digest: sha256:abc123... size: 1234567
```

---

## Step 2.5: Verify on Docker Hub

1. Go to: https://hub.docker.com/r/YOUR_DOCKER_HUB_USERNAME/studioflow
2. You should see your repository with tags `latest` and `v1.0`

✅ **Image pushed to Docker Hub successfully!**

---

# PART 3: Set Up EC2 Instance

## Step 3.1: Launch EC2 Instance

1. Go to: https://console.aws.amazon.com
2. Go to **EC2 Dashboard**
3. Click **"Launch Instances"**
4. Configure:
   - **Name:** `studioflow-api`
   - **AMI:** Search for and select `Amazon Linux 2023` (or `Amazon Linux 2` if you prefer older version)
   - **Instance Type:** `t3.small` (or `t3.medium` for production)
   - **Key Pair:** Select existing or create new
   - **Security Group:** Create new or select existing
     - **Inbound Rules:**
       - SSH (22) from your IP
       - HTTP (80) from 0.0.0.0/0
       - HTTPS (443) from 0.0.0.0/0
       - Custom TCP (3306) from 0.0.0.0/0
5. Click **"Launch Instances"**

Wait 2-3 minutes for instance to start.

---

## Step 3.2: Get Your EC2 IP Address

1. Go to EC2 Dashboard
2. Click on your instance
3. Find **"Public IPv4 address"** (e.g., `54.123.45.67`)

Keep this handy - you'll need it frequently.

---

## Step 3.3: Connect to Your EC2 Instance via SSH

Open Git Bash or PowerShell:

```bash
# Navigate to where you downloaded your SSH key
cd C:\Users\YOUR_USER\Downloads

# SSH into your instance (replace IP and key file name)
# For Amazon Linux, use 'ec2-user' instead of 'ubuntu'
ssh -i "your-key-pair.pem" ec2-user@YOUR_EC2_IP
```

**Example:**
```bash
ssh -i "studioflow-key.pem" ec2-user@54.123.45.67
```

**Expected output:**
```
Welcome to Amazon Linux 2023
ec2-user@ip-10-0-0-123 ~]$
```

---

## Step 3.4: Install Docker on EC2

```bash
# Update system packages
sudo yum update -y && sudo yum upgrade -y

# Install Docker
sudo yum install -y docker

# Start Docker
sudo systemctl start docker
sudo systemctl enable docker

# Add ec2-user to docker group (so you don't need sudo)
sudo usermod -aG docker ec2-user

# Log out and back in for group change to take effect
exit
```

SSH back in:
```bash
ssh -i "your-key-pair.pem" ec2-user@YOUR_EC2_IP
```

---

## Step 3.5: Install Docker Compose on EC2

```bash
# Download Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose

# Make it executable
sudo chmod +x /usr/local/bin/docker-compose

# Verify installation
docker-compose --version
# Expected: Docker Compose version 2.x.x or higher
```

---

## Step 3.6: Create App Directory Structure

```bash
# Create app directory
mkdir -p ~/studioflow-app
cd ~/studioflow-app

# Create subdirectories
mkdir -p ./db-data ./app-logs ./backups ./init-db

# Verify structure
ls -la
```

---

# PART 4: Deploy to EC2

## Step 4.1: Create .env File on EC2

```bash
# Create .env file
cat > .env << 'EOF'
# ===========================
# EC2 PRODUCTION ENVIRONMENT
# ===========================

# Docker Hub Image (REPLACE WITH YOUR USERNAME)
DOCKER_HUB_USERNAME=YOUR_DOCKER_HUB_USERNAME
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

# ASP.NET Core
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:80

# Logging
LOG_LEVEL=Information
EOF
```

Verify file was created:
```bash
cat .env
```

**⚠️ Important:** Replace `YOUR_DOCKER_HUB_USERNAME` with your actual Docker Hub username and `YourStrongPassword123!` with a strong password.

---

## Step 4.2: Create docker-compose.yml on EC2

```bash
cat > docker-compose.yml << 'EOF'
version: '3.8'

services:
  # MySQL Database Service
  mysql:
    image: mysql:8.0
    container_name: studioflow-mysql
    restart: always
    environment:
      MYSQL_ROOT_PASSWORD: ${DB_PASSWORD}
      MYSQL_DATABASE: ${DB_NAME}
    ports:
      - "${DB_PORT}:3306"
    volumes:
      - mysql_data:/var/lib/mysql
      - ./init-db:/docker-entrypoint-initdb.d
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
      ConnectionStrings__DefaultConnection: "server=mysql;port=3306;database=${DB_NAME};user=root;password=${DB_PASSWORD}"
      
      # API Configuration
      ASPNETCORE_ENVIRONMENT: ${ASPNETCORE_ENVIRONMENT}
      ASPNETCORE_URLS: ${ASPNETCORE_URLS}
      
      # Logging
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
      test: ["CMD", "curl", "-f", "http://localhost/health || exit 1"]
      timeout: 10s
      retries: 5
      interval: 30s

volumes:
  mysql_data:
    driver: local

networks:
  studioflow-network:
    driver: bridge
EOF
```

Verify file was created:
```bash
cat docker-compose.yml
```

---

## Step 4.3: Create Database Init Script on EC2

```bash
cat > ./init-db/01-schema.sql << 'EOF'
-- ===========================
-- StudioFlow Database Schema
-- ===========================

USE studio_db;

-- Departments Table
CREATE TABLE IF NOT EXISTS Departments (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Name VARCHAR(100) NOT NULL,
    Description VARCHAR(500) NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Users Table
CREATE TABLE IF NOT EXISTS Users (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(255) NOT NULL UNIQUE,
    Password LONGTEXT NOT NULL,
    Role INT NOT NULL,
    IsActive TINYINT(1) NOT NULL,
    CreatedAt DATETIME(6) NOT NULL,
    INDEX IX_Users_Email (Email)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Projects Table
CREATE TABLE IF NOT EXISTS Projects (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Title LONGTEXT NOT NULL,
    ArtistName LONGTEXT NOT NULL,
    Description LONGTEXT NULL,
    Deadline DATETIME(6) NULL,
    TargetReleaseDate DATETIME(6) NULL,
    Status INT NOT NULL,
    CreatedBy INT NOT NULL,
    CreatedAt DATETIME(6) NOT NULL,
    UpdatedAt DATETIME(6) NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Samples Table
CREATE TABLE IF NOT EXISTS Samples (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    ProjectId INT NOT NULL,
    Title LONGTEXT NOT NULL,
    SourceArtist LONGTEXT NULL,
    SourceTrack LONGTEXT NULL,
    RightsHolder LONGTEXT NULL,
    Status INT NOT NULL,
    CreatedAt DATETIME(6) NOT NULL,
    UpdatedAt DATETIME(6) NULL,
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE,
    INDEX IX_Samples_ProjectId (ProjectId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Clearances Table
CREATE TABLE IF NOT EXISTS Clearances (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    SampleId INT NOT NULL UNIQUE,
    RightsOwner LONGTEXT NOT NULL,
    LicenseType LONGTEXT NULL,
    IsApproved TINYINT(1) NOT NULL,
    ApprovedAt DATETIME(6) NULL,
    Notes LONGTEXT NULL,
    CreatedAt DATETIME(6) NOT NULL,
    FOREIGN KEY (SampleId) REFERENCES Samples(Id) ON DELETE CASCADE,
    INDEX IX_Clearances_SampleId (SampleId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Migrations History Table
CREATE TABLE IF NOT EXISTS __EFMigrationsHistory (
    MigrationId NVARCHAR(150) NOT NULL PRIMARY KEY,
    ProductVersion NVARCHAR(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Record Migration History
INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES 
('20260314150057_InitialCreate', '8.0.0'),
('20260315083052_CreateUsersTable', '8.0.0'),
('20260315084915_AddUniqueEmailConstraint', '8.0.0'),
('20260316021104_AddProjects', '8.0.0'),
('20260316041417_AddUpdateAtToProjects', '8.0.0'),
('20260316134817_AddSamples', '8.0.0'),
('20260316184756_AddClearanceModule', '8.0.0')
ON DUPLICATE KEY UPDATE ProductVersion=VALUES(ProductVersion);

SELECT 'Database initialization completed successfully' AS status;
EOF
```

Verify file:
```bash
cat ./init-db/01-schema.sql
```

---

## Step 4.4: Login to Docker Hub on EC2

```bash
docker login
```

When prompted:
- **Username:** Your Docker Hub username
- **Password:** Your Docker Hub password or token

---

## Step 4.5: Pull and Start Services

```bash
# Pull the latest image from Docker Hub
docker pull YOUR_DOCKER_HUB_USERNAME/studioflow:v1.0

# Example:
# docker pull john123/studioflow:v1.0

# Start all services in background
docker-compose up -d

# Expected output:
# [+] Running 3/3
# ✔ Network studioflow_studioflow-network Created
# ✔ Container studioflow-mysql Created
# ✔ Container studioflow-api Created
```

---

## Step 4.6: Check Service Status

```bash
# List all running services
docker-compose ps

# Expected output:
# NAME                STATUS               PORTS
# studioflow-mysql    Up (healthy)         0.0.0.0:3306->3306/tcp
# studioflow-api      Up (healthy)         0.0.0.0:5000->80/tcp, 0.0.0.0:5001->443/tcp
```

---

# PART 5: Verify Deployment

## Step 5.1: Check Logs

```bash
# View all logs
docker-compose logs

# View only API logs
docker-compose logs studioflow

# View only MySQL logs
docker-compose logs mysql

# View logs in real-time
docker-compose logs -f studioflow
```

Look for "Application started" message in logs.

---

## Step 5.2: Test Database Connection

```bash
# Connect to MySQL
docker exec -it studioflow-mysql mysql -uroot -p${DB_PASSWORD} -e "SHOW TABLES FROM studio_db;"

# Expected output:
# +-----------------------------+
# | Tables_in_studio_db         |
# +-----------------------------+
# | __EFMigrationsHistory       |
# | Clearances                  |
# | Departments                 |
# | Projects                    |
# | Samples                      |
# | Users                        |
# +-----------------------------+
```

✅ If you see all tables, database is set up correctly!

---

## Step 5.3: Test API from EC2 (Local)

```bash
# Test health endpoint
curl http://localhost:5000/api/users

# Test from different terminal on your local machine
curl http://YOUR_EC2_IP:5000/api/users

# Example:
# curl http://54.123.45.67:5000/api/users
```

---

## Step 5.4: Test API from Postman or Browser

1. Open Postman or your browser
2. Go to: `http://YOUR_EC2_IP:5000/api/users`
3. You should get a response (may be empty array or require auth)

---

## Step 5.5: Test Creating a User

Using Postman or curl:

**POST** `http://YOUR_EC2_IP:5000/api/users`

**Headers:**
```
Content-Type: application/json
```

**Body:**
```json
{
  "name": "Test User",
  "email": "test@example.com",
  "password": "TestPass123!",
  "role": "PRODUCER"
}
```

**Expected response:** 201 Created with user data

---

## Step 5.6: Verify Volumes

```bash
# Check volumes
docker volume ls

# Check database data
docker exec studioflow-mysql ls -la /var/lib/mysql

# Check application logs
docker-compose exec studioflow ls -la /app/logs
```

---

# 🎉 Deployment Complete!

## Quick Reference

| Task | Command |
|------|---------|
| **View Status** | `docker-compose ps` |
| **View Logs** | `docker-compose logs -f studioflow` |
| **Stop Services** | `docker-compose stop` |
| **Start Services** | `docker-compose start` |
| **Restart All** | `docker-compose restart` |
| **View Database** | `docker exec -it studioflow-mysql mysql -uroot -p${DB_PASSWORD}` |
| **Access API** | `http://YOUR_EC2_IP:5000` |

---

## Important Notes

### Security
- ⚠️ **Change DB password in .env** - Don't use `sachin1605` in production
- ⚠️ **Restrict SSH access** - Use security groups instead of 0.0.0.0/0
- ⚠️ **Use HTTPS** - Set up SSL certificate for production

### Backups
```bash
# Create database backup
docker exec studioflow-mysql mysqldump -uroot -p${DB_PASSWORD} studio_db > backup-$(date +%Y%m%d-%H%M%S).sql

# Restore from backup
docker exec -i studioflow-mysql mysql -uroot -p${DB_PASSWORD} studio_db < backup-file.sql
```

### Update Application
```bash
# Build and push new version
docker build -t studioflow:v1.1 .
docker tag studioflow:v1.1 YOUR_DOCKER_HUB_USERNAME/studioflow:v1.1
docker push YOUR_DOCKER_HUB_USERNAME/studioflow:v1.1

# Update on EC2
# Edit .env and change: DOCKER_IMAGE_TAG=v1.1
# Then: docker-compose up -d
```

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| **Can't SSH to EC2** | Check key pair, security group allows port 22, IP is correct |
| **Services won't start** | Check logs: `docker-compose logs` |
| **Database error** | Check password in .env matches docker-compose.yml |
| **Can't reach API from browser** | Check security group allows port 5000, check API logs |
| **Out of disk space** | Run `docker system prune -a` to clean up unused images |

---

## Next Steps

1. ✅ Test all API endpoints
2. ✅ Set up automated backups
3. ✅ Configure HTTPS/SSL
4. ✅ Set up monitoring and alerts
5. ✅ Plan scaling strategy

---

**Status:** ✅ Complete End-to-End Guide  
**Updated:** March 17, 2026  
**Tested:** Working on EC2 Amazon Linux 2023

