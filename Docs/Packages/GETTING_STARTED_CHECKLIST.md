# ✅ Docker Hub & EC2 Deployment - Getting Started Checklist

**Status:** ✅ Complete & Ready  
**Date:** March 17, 2026  
**Time to Complete:** 30-45 minutes

---

## 🎯 Pre-Deployment Checklist

### ✓ Local Machine Setup (Windows)

- [ ] Docker Desktop installed
  - Download from: https://www.docker.com/products/docker-desktop
  - Or verify: `docker --version`

- [ ] Docker running
  - Start Docker Desktop
  - Verify: `docker ps` returns output

- [ ] Git Bash or PowerShell available
  - For running scripts
  - Recommend: Windows Terminal + PowerShell

- [ ] Project directory accessible
  - Path: `C:\Users\User\Desktop\Programming\ASP\StudioFlow`
  - Verify: Files present

- [ ] Docker Hub account created
  - Go to: https://hub.docker.com
  - Create free account if needed
  - Note your username

- [ ] AWS Account (if EC2 not yet created)
  - Go to: https://aws.amazon.com
  - Create account if needed
  - Set up billing

---

### ✓ AWS EC2 Preparation

- [ ] EC2 instance running
  - **AMI:** Ubuntu 22.04 LTS (ami-0c55b159cbfafe1f0)
  - **Instance Type:** t3.small (minimum)
  - **Storage:** 20 GB minimum
  - **Availability Zone:** Any in your region

- [ ] Security groups configured
  - Port 22 (SSH) - from your IP
  - Port 80 (HTTP) - from 0.0.0.0/0
  - Port 443 (HTTPS) - from 0.0.0.0/0 (future)
  - Port 3306 (MySQL) - from internal only

- [ ] SSH key pair available
  - File: your-key-pair.pem
  - Permissions: `chmod 400 your-key.pem`
  - Location: Safe and backed up

- [ ] EC2 instance IP address noted
  - Public IP: `xxx.xxx.xxx.xxx`
  - Elastic IP recommended for production

- [ ] SSH connection tested
  - Command: `ssh -i "your-key.pem" ubuntu@your-ec2-ip`
  - Result: Connected successfully

---

## 📚 Documentation Review

### ✓ Read These (In Order)

1. [ ] **DEPLOYMENT_SUMMARY.md** (5 minutes)
   - Understand what's been prepared
   - See architecture diagrams
   - Note file locations

2. [ ] **QUICK_START_DEPLOYMENT.md** (10 minutes)
   - Read: Prerequisites section
   - Read: Phase 1 (Push to Docker Hub)
   - Note: Exact commands

3. [ ] **DOCKER_DEPLOYMENT_GUIDE.md** (optional, reference)
   - Read: As needed during deployment
   - Bookmark: Troubleshooting section

4. [ ] **DEPLOYMENT_INDEX.md** (optional, reference)
   - Use: For quick command lookup
   - Reference: Command cheatsheet

---

## 🔧 Files Available

### ✓ Verify Files Present

- [ ] **Documentation Files** (5 files)
  - [ ] QUICK_START_DEPLOYMENT.md
  - [ ] DOCKER_DEPLOYMENT_GUIDE.md
  - [ ] DEPLOYMENT_INDEX.md
  - [ ] DEPLOYMENT_SUMMARY.md
  - [ ] FILE_MANIFEST_DOCKER_EC2.md

- [ ] **Deployment Scripts** (3 files)
  - [ ] push-to-docker-hub.ps1
  - [ ] push-to-docker-hub.sh
  - [ ] setup-ec2.sh

- [ ] **Configuration Files**
  - [ ] docker-compose.ec2.yml
  - [ ] .env.example

- [ ] **Existing Docker Files**
  - [ ] Dockerfile
  - [ ] docker-compose.yml

---

## 🚀 Phase 1: Push to Docker Hub (5-10 minutes)

### ✓ Prepare Local Machine

- [ ] Verify Docker running
  ```powershell
  docker ps
  ```

- [ ] Verify project files
  ```powershell
  cd C:\Users\User\Desktop\Programming\ASP\StudioFlow
  ls Dockerfile
  ```

- [ ] Have Docker Hub username ready
  - Open: https://hub.docker.com/settings/profile
  - Note username (not email)

### ✓ Run Push Script

- [ ] Execute (Windows PowerShell):
  ```powershell
  .\push-to-docker-hub.ps1 -Tag v1.0 -DockerUsername yourusername
  ```

- [ ] Monitor progress:
  - Step 1/6: Configuration
  - Step 2/6: Verify Docker
  - Step 3/6: Check Dockerfile
  - Step 4/6: Build image (3-5 min)
  - Step 5/6: Authenticate
  - Step 6/6: Push to Docker Hub (2-5 min)

### ✓ Verify Push Success

- [ ] Check console output
  - Look for: "✅ Push Successful!"
  - Note: Docker Hub URL provided

- [ ] Visit Docker Hub
  - Go to: https://hub.docker.com/r/yourusername/studioflow
  - Verify: Image present
  - Verify: Tags shown (v1.0, latest)
  - Verify: Size ~650 MB

- [ ] Note for later:
  - Docker Hub username: _______________
  - Image tag: v1.0
  - Docker Hub URL: https://hub.docker.com/r/yourusername/studioflow

---

## 🖥️ Phase 2: EC2 Setup (5-10 minutes)

### ✓ SSH to EC2 Instance

- [ ] Open Terminal/PowerShell

- [ ] SSH command:
  ```bash
  ssh -i "your-key.pem" ubuntu@your-ec2-ip
  ```

- [ ] Verify connected:
  - Prompt shows: `ubuntu@ip-xxx-xxx-xxx-xxx:~$`

- [ ] Verify Linux version:
  ```bash
  uname -a
  # Should show: Ubuntu
  ```

### ✓ Download Setup Script

- [ ] Create directory:
  ```bash
  mkdir -p ~/studioflow-app
  cd ~/studioflow-app
  ```

- [ ] Option A - Copy via SCP (from local machine):
  ```bash
  scp -i "your-key.pem" setup-ec2.sh ubuntu@your-ec2-ip:~/studioflow-app/
  ```

- [ ] Option B - Create manually on EC2:
  ```bash
  # Copy content of setup-ec2.sh and paste
  nano setup-ec2.sh
  # Paste content, save with Ctrl+X, Y, Enter
  ```

- [ ] Verify file:
  ```bash
  ls -la setup-ec2.sh
  ```

### ✓ Run Setup Script

- [ ] Make executable:
  ```bash
  chmod +x setup-ec2.sh
  ```

- [ ] Run script:
  ```bash
  bash setup-ec2.sh
  ```

- [ ] Follow prompts:
  - Step 1/6: Verify prerequisites
  - Step 2/6: Create directories
  - Step 3/6: Create .env
  - Step 4/6: Create docker-compose.yml
  - Step 5/6: Docker login prompt (say yes if first time)
  - Step 6/6: Setup complete

- [ ] Verify directories created:
  ```bash
  ls -la ~/studioflow-app/
  # Should show: db-data, app-logs, backups, init-db, docker-compose.yml, .env
  ```

---

## ⚙️ Phase 3: Configure & Deploy (10-15 minutes)

### ✓ Edit Configuration

- [ ] Edit .env file:
  ```bash
  cd ~/studioflow-app
  nano .env
  ```

- [ ] Update critical values:
  ```bash
  DOCKER_HUB_USERNAME=yourusername        # Your Docker Hub username
  DOCKER_IMAGE_TAG=v1.0                   # Same as pushed
  DB_PASSWORD=YourStrongPassword123!      # Change this!
  ASPNETCORE_ENVIRONMENT=Production       # Production mode
  ```

- [ ] Save file:
  - Press: Ctrl+X
  - Press: Y
  - Press: Enter

- [ ] Verify changes:
  ```bash
  cat .env | grep DOCKER_HUB_USERNAME
  cat .env | grep DB_PASSWORD
  ```

### ✓ Prepare Docker Compose

- [ ] Review docker-compose.yml:
  ```bash
  nano docker-compose.yml
  # Just review, don't change for now
  # Press Ctrl+X to exit
  ```

- [ ] Verify image reference:
  ```bash
  grep "image:" docker-compose.yml
  # Should reference your image
  ```

### ✓ Start Services

- [ ] Navigate to app directory:
  ```bash
  cd ~/studioflow-app
  ```

- [ ] Login to Docker (first time only):
  ```bash
  docker login
  # Enter Docker Hub username
  # Enter Docker Hub password/token
  # Should see: Login Succeeded
  ```

- [ ] Pull image:
  ```bash
  docker pull yourusername/studioflow:v1.0
  # This downloads the image from Docker Hub
  # May take 2-5 minutes
  ```

- [ ] Start services:
  ```bash
  docker-compose up -d
  # Services start in background
  # Wait 30 seconds for initialization
  ```

- [ ] Check status:
  ```bash
  docker-compose ps
  # Expected output:
  # NAME            STATUS        PORTS
  # studioflow-mysql    Up (healthy)  3306/tcp
  # studioflow-api      Up (healthy)  5000:80/tcp
  ```

---

## ✅ Phase 4: Verification (5 minutes)

### ✓ Container Status

- [ ] All containers healthy:
  ```bash
  docker-compose ps
  # Both should show "Up"
  # Both should show "healthy"
  ```

- [ ] View logs (ensure no errors):
  ```bash
  docker-compose logs
  # Should NOT show critical errors
  # Look for: "Application started"
  ```

### ✓ Test API Endpoint

- [ ] From EC2 local:
  ```bash
  curl http://localhost:5000/api/clearances
  # Should return JSON array (may be empty initially)
  ```

- [ ] From your local machine:
  ```bash
  # PowerShell or Bash
  curl http://your-ec2-ip:5000/api/clearances
  # Or via browser:
  # http://your-ec2-ip:5000/api/clearances
  ```

- [ ] Expected response:
  - Status: 200 OK
  - Body: JSON array `[]` or with data

### ✓ Database Access

- [ ] From EC2:
  ```bash
  docker exec -it studioflow-mysql mysql -uroot -p${DB_PASSWORD}
  # At prompt: SHOW DATABASES;
  # Should list: information_schema, mysql, performance_schema, studio_db
  # Exit: exit
  ```

### ✓ View Application Logs

- [ ] Check API logs:
  ```bash
  docker-compose logs -f studioflow
  # View real-time logs
  # Press Ctrl+C to exit
  ```

- [ ] Check database logs:
  ```bash
  docker-compose logs -f mysql
  # View database logs
  # Press Ctrl+C to exit
  ```

---

## 🧪 Testing (Optional but Recommended)

### ✓ Test Clearance Feature

- [ ] From your local machine, test with Postman or curl:

1. **Create Sample** (if doesn't exist):
   ```bash
   curl -X POST http://your-ec2-ip:5000/api/samples \
     -H "Content-Type: application/json" \
     -d '{
       "projectId": 1,
       "fileName": "test-sample.wav",
       "audioUrl": "https://example.com/audio.wav",
       "duration": 30,
       "uploadedBy": "user1"
     }'
   ```

2. **Create Clearance**:
   ```bash
   curl -X POST http://your-ec2-ip:5000/api/clearances \
     -H "Content-Type: application/json" \
     -d '{
       "sampleId": 1,
       "rightsOwner": "Artist Name",
       "licenseType": "CC BY 4.0"
     }'
   ```

3. **Get Clearances**:
   ```bash
   curl http://your-ec2-ip:5000/api/clearances
   ```

---

## 📋 Post-Deployment Tasks

### ✓ Backup Configuration

- [ ] Create backup script:
  ```bash
  cd ~/studioflow-app
  cat > backup-db.sh << 'EOF'
  #!/bin/bash
  DATE=$(date +%Y%m%d-%H%M%S)
  docker exec studioflow-mysql mysqldump -uroot -p${DB_PASSWORD} studio_db > ./backups/backup-$DATE.sql
  echo "Backup created: backup-$DATE.sql"
  EOF
  ```

- [ ] Make executable:
  ```bash
  chmod +x backup-db.sh
  ```

- [ ] Test backup:
  ```bash
  ./backup-db.sh
  # Should create backup file
  ls -lah ./backups/
  ```

### ✓ Setup Monitoring

- [ ] Check resource usage:
  ```bash
  docker stats
  # View container CPU and memory usage
  # Press Ctrl+C to exit
  ```

### ✓ Document Deployment

- [ ] Note down:
  - EC2 IP Address: _______________
  - Docker Hub Username: _______________
  - Database Password: ✓ Stored securely
  - SSH Key Location: _______________

---

## 🔐 Security Verification

- [ ] .env file has restricted permissions:
  ```bash
  ls -l .env
  # Should show: -rw------- (600)
  ```

- [ ] .env not in Git:
  ```bash
  cat .gitignore | grep .env
  # Should show: .env
  ```

- [ ] Database password is strong:
  - [ ] 20+ characters
  - [ ] Mix of upper, lower, numbers, special chars
  - [ ] Not in any documentation
  - [ ] Only in .env file

- [ ] SSH key is secure:
  - [ ] Located safely
  - [ ] Permissions: 400
  - [ ] Not shared with anyone
  - [ ] Backed up

---

## 🚨 If Something Goes Wrong

### ✓ Common Issues Checklist

1. **"Connection refused" when accessing API**
   - [ ] Verify services running: `docker-compose ps`
   - [ ] Check firewall: EC2 security groups allow port 80/5000
   - [ ] Check logs: `docker-compose logs studioflow`

2. **"Database connection failed"**
   - [ ] Check DB_PASSWORD matches .env
   - [ ] Check MySQL running: `docker-compose ps mysql`
   - [ ] View MySQL logs: `docker-compose logs mysql`

3. **"Image not found"**
   - [ ] Verify Docker Hub login: `docker login`
   - [ ] Verify image name matches
   - [ ] Try pulling again: `docker pull yourusername/studioflow:v1.0`

4. **"Out of disk space"**
   - [ ] Check space: `df -h`
   - [ ] Clean up: `docker system prune -a`

**For more issues, consult:** DOCKER_DEPLOYMENT_GUIDE.md → Troubleshooting

---

## ✅ Final Verification Checklist

- [ ] All files created and present
- [ ] Documentation reviewed
- [ ] Docker image pushed to Docker Hub
- [ ] EC2 instance running
- [ ] Docker installed on EC2
- [ ] .env configured with credentials
- [ ] docker-compose.yml ready
- [ ] Services started: `docker-compose up -d`
- [ ] All containers showing "Up"
- [ ] API responding to requests
- [ ] Database accessible
- [ ] Backups working
- [ ] Logs available
- [ ] Security checks passed

---

## 🎉 Deployment Complete!

When all checks are ✅, your deployment is complete!

**What's Running:**
- ✅ MySQL 8.0 on port 3306 (internal)
- ✅ StudioFlow API on port 80/5000
- ✅ Persistent database volume
- ✅ Application logs volume
- ✅ Health checks active
- ✅ Auto-restart on failure

**You Can Now:**
- ✅ Access API at: `http://your-ec2-ip:5000`
- ✅ View logs: `docker-compose logs -f`
- ✅ Access database: `docker exec -it studioflow-mysql mysql -uroot -p`
- ✅ Create backups: `./backup-db.sh`
- ✅ Update to new version: Update .env tag, pull image, restart

---

## 📞 Need Help?

| Problem | Solution |
|---------|----------|
| Quick overview | Read: QUICK_START_DEPLOYMENT.md |
| Step-by-step help | Read: DOCKER_DEPLOYMENT_GUIDE.md |
| Command reference | Read: DEPLOYMENT_INDEX.md |
| Architecture details | Read: DEPLOYMENT_SUMMARY.md |
| File information | Read: FILE_MANIFEST_DOCKER_EC2.md |

---

## 📊 Summary

| Phase | Time | Status |
|-------|------|--------|
| 1. Push to Docker Hub | 5-10 min | ⏳ Next |
| 2. EC2 Setup | 5-10 min | ⏳ Next |
| 3. Configure & Deploy | 10-15 min | ⏳ Next |
| 4. Verification | 5 min | ⏳ Next |
| **Total** | **30-45 min** | **⏳ Start Now!** |

---

**Status:** ✅ READY TO DEPLOY  
**Generated:** March 17, 2026  
**Next Step:** Follow Phase 1 above!

🚀 **LET'S GO!** 🚀

