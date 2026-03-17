# 🔄 AWS Linux vs Ubuntu Deployment Guide

**Quick Reference: Key Differences**

---

## 📌 Main Differences

| Aspect | Ubuntu 22.04 | Amazon Linux 2023 |
|--------|--------------|-------------------|
| **Default User** | `ubuntu` | `ec2-user` |
| **Package Manager** | `apt` | `yum` |
| **Update Command** | `apt update && apt upgrade` | `yum update && yum upgrade` |
| **Install Docker** | `apt install docker.io` | `yum install docker` |
| **Docker Package** | `docker.io` | `docker` |
| **Init System** | `systemd` | `systemd` |
| **Home Directory** | `/home/ubuntu` | `/home/ec2-user` |

---

## 🔧 Command Translation

### Update System Packages

**Ubuntu:**
```bash
sudo apt update && sudo apt upgrade -y
```

**Amazon Linux:**
```bash
sudo yum update -y && sudo yum upgrade -y
```

---

### Install Docker

**Ubuntu:**
```bash
sudo apt install -y docker.io
```

**Amazon Linux:**
```bash
sudo yum install -y docker
```

---

### Add User to Docker Group

**Ubuntu:**
```bash
sudo usermod -aG docker ubuntu
```

**Amazon Linux:**
```bash
sudo usermod -aG docker ec2-user
```

---

### SSH into Instance

**Ubuntu:**
```bash
ssh -i "key.pem" ubuntu@54.123.45.67
```

**Amazon Linux:**
```bash
ssh -i "key.pem" ec2-user@54.123.45.67
```

---

### Navigate to Home Directory

**Ubuntu:**
```bash
cd /home/ubuntu
cd ~
```

**Amazon Linux:**
```bash
cd /home/ec2-user
cd ~
```

---

## 📝 Installation Script for Amazon Linux

Save this as `setup-docker.sh` and run on your EC2 instance:

```bash
#!/bin/bash

echo "🚀 Setting up Docker on Amazon Linux..."

# Update system
echo "📦 Updating packages..."
sudo yum update -y && sudo yum upgrade -y

# Install Docker
echo "🐳 Installing Docker..."
sudo yum install -y docker

# Start Docker
echo "▶️  Starting Docker service..."
sudo systemctl start docker
sudo systemctl enable docker

# Add ec2-user to docker group
echo "👤 Adding ec2-user to docker group..."
sudo usermod -aG docker ec2-user

# Install Docker Compose
echo "📦 Installing Docker Compose..."
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Verify installations
echo "✅ Verifying installations..."
docker --version
docker-compose --version

echo "✅ Setup complete! Please log out and back in for docker group changes to take effect."
echo "   Then: ssh -i 'your-key.pem' ec2-user@YOUR_EC2_IP"
```

**Run it:**
```bash
chmod +x setup-docker.sh
./setup-docker.sh
```

---

## 🎯 Quick Start for Amazon Linux

### 1️⃣ Connect to EC2
```bash
ssh -i "studioflow-key.pem" ec2-user@54.123.45.67
```

### 2️⃣ Create App Directory
```bash
mkdir -p ~/studioflow-app/init-db
cd ~/studioflow-app
```

### 3️⃣ Create .env File
```bash
cat > .env << 'EOF'
DOCKER_HUB_USERNAME=your_username
DOCKER_IMAGE_TAG=v1.0
DB_HOST=mysql
DB_PORT=3306
DB_NAME=studio_db
DB_USER=root
DB_PASSWORD=YourStrongPassword123!
API_PORT=5000
API_HTTPS_PORT=5001
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:80
LOG_LEVEL=Information
EOF
```

### 4️⃣ Create Database Init Script
```bash
cat > ./init-db/01-schema.sql << 'EOF'
USE studio_db;
-- [Paste SQL schema from main guide]
EOF
```

### 5️⃣ Create docker-compose.yml
```bash
# Copy from main guide's Step 4.2
cat > docker-compose.yml << 'EOF'
# [Paste docker-compose content from main guide]
EOF
```

### 6️⃣ Login and Pull
```bash
docker login
docker pull your_username/studioflow:v1.0
docker-compose up -d
```

### 7️⃣ Verify
```bash
docker-compose ps
docker-compose logs -f studioflow
```

---

## ⚡ Useful Amazon Linux Commands

```bash
# Check OS version
cat /etc/os-release

# Check available yum repositories
yum repolist

# Install a package
sudo yum install -y package-name

# Search for a package
yum search package-name

# Remove a package
sudo yum remove -y package-name

# Check running services
sudo systemctl list-units --type=service --state=running

# View system logs
sudo journalctl -xe

# Check disk usage
df -h
du -sh ~/*
```

---

## 🐛 Troubleshooting on Amazon Linux

### Docker command not found after adding to group

**Problem:** You added `ec2-user` to docker group but still get "permission denied"

**Solution:**
```bash
# Log out
exit

# Log back in (super important!)
ssh -i "key.pem" ec2-user@YOUR_IP

# Verify group membership
groups
# Should show: ec2-user : ec2-user docker
```

### yum: command not found (shouldn't happen, but just in case)

```bash
# Try without sudo first
yum update -y

# If that doesn't work, ensure you're using Amazon Linux
cat /etc/os-release
```

### Docker service won't start

```bash
# Check status
sudo systemctl status docker

# Check logs
sudo journalctl -u docker -n 50

# Try restarting
sudo systemctl restart docker
```

### Port already in use

```bash
# Find what's using port 5000
sudo lsof -i :5000

# Kill the process
sudo kill -9 <PID>
```

---

## 📚 Amazon Linux Documentation

- Official Docs: https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/amazon-linux-2.html
- Package Manager: https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/manage-packages.html
- Docker on Amazon Linux: https://docs.aws.amazon.com/AmazonECS/latest/developerguide/docker-basics.html

---

## ✅ Verification Checklist

- [ ] EC2 instance launched with Amazon Linux 2023 AMI
- [ ] SSH connection working with `ec2-user` user
- [ ] Docker installed and running (`docker ps`)
- [ ] Docker Compose installed (`docker-compose --version`)
- [ ] User added to docker group (no `sudo` needed for docker commands)
- [ ] `.env` file created with correct values
- [ ] `docker-compose.yml` created
- [ ] Database init script in place
- [ ] Image pulled from Docker Hub
- [ ] Services started successfully (`docker-compose up -d`)
- [ ] API responding (`curl http://localhost:5000/api/users`)

---

**Reference Guide:** Updated March 17, 2026  
**Target Platform:** AWS Linux 2 & 2023  
**Status:** ✅ Ready for Production

