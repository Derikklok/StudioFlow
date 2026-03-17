# 🚀 Quick Start - Push to Docker Hub & Deploy on EC2

**Status:** ✅ Ready to Deploy  
**Time Required:** 30-45 minutes

---

## 📋 Quick Overview

```
Step 1: Push Image
    ↓
Step 2: EC2 Setup
    ↓
Step 3: Run Services
    ↓
Step 4: Verify
```

---

## ✅ Prerequisites Checklist

- [ ] Docker Desktop installed on Windows
- [ ] Docker Hub account created (hub.docker.com)
- [ ] EC2 instance running (Ubuntu 22.04 LTS)
- [ ] SSH access to EC2 instance
- [ ] Project built and compiles successfully

---

## 🚀 STEP 1: Push Image to Docker Hub (On Windows)

### 1A. Open PowerShell or Git Bash

Navigate to project directory:
```bash
cd C:\Users\User\Desktop\Programming\ASP\StudioFlow
```

### 1B. Run Push Script

```bash
# Make script executable (if needed)
bash push-to-docker-hub.sh

# Or push with specific version
bash push-to-docker-hub.sh v1.0
```

**What it does:**
- ✅ Builds Docker image
- ✅ Tags with version
- ✅ Authenticates to Docker Hub
- ✅ Pushes image
- ✅ Shows Docker Hub URL

**Expected Output:**
```
The push refers to repository [docker.io/yourusername/studioflow]
v1.0: digest: sha256:abc123... size: 1234
latest: digest: sha256:abc123... size: 1234
✅ Push Successful!
```

### 1C. Verify on Docker Hub

Visit: `https://hub.docker.com/r/yourusername/studioflow`

You should see:
- ✅ Repository created
- ✅ Tags: v1.0, latest
- ✅ Image size: ~650MB

---

## 🖥️ STEP 2: EC2 Setup (On Linux/EC2)

### 2A. SSH into EC2

```bash
ssh -i "your-key.pem" ubuntu@your-ec2-ip
```

### 2B. Download Setup Script

```bash
# Create working directory
mkdir -p ~/studioflow-app
cd ~/studioflow-app

# Download setup script from your local machine or Git
# Option 1: Copy from local via SCP
scp -i "your-key.pem" setup-ec2.sh ubuntu@your-ec2-ip:~/studioflow-app/

# Option 2: Create manually (copy the setup-ec2.sh content)
nano setup-ec2.sh
# Then paste the content
```

### 2C. Run Setup Script

```bash
# Make executable
chmod +x setup-ec2.sh

# Run setup
bash setup-ec2.sh

# Follow prompts
```

**What it does:**
- ✅ Verifies Docker installed
- ✅ Installs Docker Compose if needed
- ✅ Creates directory structure
- ✅ Creates .env file template
- ✅ Creates docker-compose.yml

---

## ⚙️ STEP 3: Configure & Run Services

### 3A. Configure Environment

```bash
# Edit .env file
nano .env

# Update these values:
DOCKER_HUB_USERNAME=your-actual-username
DOCKER_IMAGE_TAG=v1.0
DB_PASSWORD=YourStrongPassword123!
ASPNETCORE_ENVIRONMENT=Production
```

**Save with:** `Ctrl+X` → `Y` → `Enter`

### 3B. Login to Docker (First Time Only)

```bash
# Login with your Docker Hub credentials
docker login

# Username: your-docker-hub-username
# Password: your-docker-hub-password (or token)
```

### 3C. Start Services

```bash
# Navigate to app directory
cd ~/studioflow-app

# Pull the latest image
docker pull your-username/studioflow:v1.0

# Start all services
docker-compose up -d

# Wait 30 seconds for database to initialize...
```

### 3D. Monitor Startup

```bash
# Check status (run every 10 seconds)
watch -n 10 docker-compose ps

# Or manually check
docker-compose ps

# Should show all containers as Up
```

---

## ✅ STEP 4: Verification

### 4A. Check Service Status

```bash
# All services should show "Up"
docker-compose ps

# Expected output:
# NAME              STATUS          PORTS
# studioflow-mysql  Up (healthy)    3306/tcp
# studioflow-api    Up (healthy)    5000:80/tcp
```

### 4B. Test API Endpoint

```bash
# Test health check
curl http://localhost:5000/api/health

# Or from another machine
curl http://your-ec2-ip:5000/api/health

# Expected response: {"status":"healthy"} or similar
```

### 4C. Test Database Connection

```bash
# Access MySQL
docker exec -it studioflow-mysql mysql -uroot -p${DB_PASSWORD}

# In MySQL:
SHOW DATABASES;
USE studio_db;
SHOW TABLES;
exit
```

### 4D. View Logs

```bash
# API logs
docker-compose logs -f studioflow

# Database logs
docker-compose logs -f mysql

# All logs
docker-compose logs -f

# Last 100 lines
docker-compose logs --tail 100
```

---

## 🧪 Test Your Deployment

### Option 1: Using cURL

```bash
# Create a clearance (adjust sample ID)
curl -X POST http://your-ec2-ip:5000/api/clearances \
  -H "Content-Type: application/json" \
  -d '{
    "sampleId": 1,
    "rightsOwner": "Artist Name",
    "licenseType": "CC BY 4.0"
  }'

# Get clearances
curl http://your-ec2-ip:5000/api/clearances

# Get specific clearance
curl http://your-ec2-ip:5000/api/clearances/1
```

### Option 2: Using Postman

1. Import: `StudioFlow_Clearance_API.postman_collection.json`
2. Update base URL: `http://your-ec2-ip:5000`
3. Run requests
4. Verify responses

### Option 3: Using Browser

```
http://your-ec2-ip:5000/api/clearances
```

Should return JSON list of clearances

---

## 🐛 Common Issues & Fixes

### Issue: "Cannot connect to Docker daemon"

**Solution:**
```bash
sudo systemctl start docker
docker --version
```

### Issue: "Image not found"

**Solution:**
```bash
# Verify login
docker login

# Verify image name
docker search your-username/studioflow

# Pull again
docker pull your-username/studioflow:v1.0
```

### Issue: "Port 5000 already in use"

**Solution:**
```bash
# Edit .env
nano .env

# Change API_PORT=5000 to API_PORT=8000
# Then restart
docker-compose restart studioflow
```

### Issue: "Database connection failed"

**Solution:**
```bash
# Check MySQL is running
docker-compose logs mysql

# Check password in .env matches
docker-compose config | grep DB_PASSWORD

# Restart database
docker-compose restart mysql

# Wait 30 seconds, then restart API
docker-compose restart studioflow
```

---

## 📊 Post-Deployment Tasks

### Backup Script

Create `~/studioflow-app/backup-db.sh`:

```bash
#!/bin/bash
DATE=$(date +%Y%m%d-%H%M%S)
docker exec studioflow-mysql mysqldump -uroot -p${DB_PASSWORD} studio_db > ./backups/backup-$DATE.sql
echo "Backup created: backup-$DATE.sql"
```

Make executable:
```bash
chmod +x backup-db.sh
```

Run backup:
```bash
./backup-db.sh
```

### Setup Daily Backup

```bash
# Edit crontab
crontab -e

# Add this line (runs backup daily at 2 AM):
0 2 * * * /home/ubuntu/studioflow-app/backup-db.sh
```

### Update Database with Migrations (If Needed)

```bash
# If you have pending migrations, the container will run them automatically
# To verify:
docker-compose logs studioflow | grep -i migration

# Or manually run migrations in container:
docker exec studioflow dotnet ef database update
```

---

## 🔐 Security Checklist

- [ ] Changed DB_PASSWORD to strong password
- [ ] Set .env file permissions: `chmod 600 .env`
- [ ] Don't commit .env to Git: `echo ".env" >> .gitignore`
- [ ] Use IAM roles for AWS (don't hardcode credentials)
- [ ] Enable HTTPS in production (use Let's Encrypt)
- [ ] Regular backups enabled
- [ ] Monitor logs for errors

---

## 📈 Performance Monitoring

```bash
# View container resource usage
docker stats

# Specific container
docker stats studioflow-api

# View persistent data
ls -lah ~/studioflow-app/db-data

# Database size
docker exec studioflow-mysql \
  du -sh /var/lib/mysql/studio_db
```

---

## 🔄 Update to New Version

When you push a new version:

```bash
# On local machine
bash push-to-docker-hub.sh v1.1

# On EC2
cd ~/studioflow-app

# Update .env
nano .env  # Change DOCKER_IMAGE_TAG=v1.1

# Pull new image
docker pull your-username/studioflow:v1.1

# Restart services
docker-compose up -d

# View logs
docker-compose logs -f
```

---

## 🧹 Cleanup (If Needed)

### Stop Services (Keep Data)

```bash
docker-compose stop
```

### Stop and Remove Containers (Keep Data)

```bash
docker-compose down
```

### Stop and Remove EVERYTHING (⚠️ Deletes Database)

```bash
docker-compose down -v
```

### Remove Old Images

```bash
docker image prune -a
```

---

## 📞 Useful Commands Reference

| Task | Command |
|------|---------|
| Start all services | `docker-compose up -d` |
| Stop all services | `docker-compose down` |
| View all logs | `docker-compose logs -f` |
| Check status | `docker-compose ps` |
| Restart service | `docker-compose restart studioflow` |
| Access MySQL | `docker exec -it studioflow-mysql mysql -uroot -p` |
| View volumes | `docker volume ls` |
| Clean up | `docker system prune -a` |
| Pull new image | `docker pull your-username/studioflow:v1.0` |
| Backup database | `./backup-db.sh` |

---

## ✨ Success Indicators

✅ All containers show "Up" in `docker-compose ps`  
✅ API responds to HTTP requests  
✅ MySQL is accessible  
✅ Logs show no errors  
✅ Database connections successful  
✅ Clearance feature working  

---

## 🎯 What's Next?

1. ✅ Test all API endpoints
2. ✅ Configure HTTPS (Let's Encrypt)
3. ✅ Set up CloudWatch monitoring
4. ✅ Create backup strategy
5. ✅ Document deployment process for team
6. ✅ Plan scaling strategy

---

## 📞 Support

**Docker Issues:** `docker logs container-name`  
**Database Issues:** Access MySQL container and check tables  
**API Issues:** Check `docker-compose logs studioflow`  
**Network Issues:** Verify EC2 security group allows ports 5000, 3306  

---

**Generated:** March 17, 2026  
**Status:** ✅ Production Ready  
**Estimated Time:** 30-45 minutes

