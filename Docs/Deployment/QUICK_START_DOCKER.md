# 🚀 Docker Quick Start Guide

Get StudioFlow running with Docker Compose in 5 minutes!

---

## ⚡ Quick Start (Local Testing)

### 1. Prerequisites Check

```bash
# Verify Docker is installed
docker --version

# Verify Docker Compose is installed
docker-compose --version

# Verify you're in the project directory
cd ~/Desktop/Programming/ASP/StudioFlow
pwd
```

### 2. Start Services

```bash
# Pull base images and build
docker-compose up -d

# Check status
docker-compose ps

# Expected output:
# NAME                STATUS
# studioflow-mysql    Up (healthy)
# studioflow-api      Up (healthy)
```

### 3. Verify Application

```bash
# Test API health
curl http://localhost:5000/health

# Expected: 200 OK

# View logs
docker-compose logs studioflow

# View database logs
docker-compose logs mysql
```

### 4. Test API Endpoint

```bash
# Create a test record
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test User",
    "email": "test@example.com",
    "password": "password123",
    "role": "Admin"
  }'

# Expected: 201 Created
```

### 5. Stop Services

```bash
# Stop all services (keeps data)
docker-compose down

# Stop and remove volumes (deletes data)
docker-compose down -v
```

---

## 📁 File Locations

| File | Purpose | Location |
|------|---------|----------|
| **docker-compose.yml** | Local/Dev setup | Root directory |
| **.env** | Environment variables | Root directory |
| **.env.example** | Template | Root directory |
| **.env.aws** | AWS production | Root directory |
| **Dockerfile** | Build instructions | Root directory |
| **Documentation** | Detailed guides | `/Docs/Deployment/` |

---

## 🔧 Configuration

### Edit Environment Variables

```bash
# Edit .env file
nano .env

# Variables you can change:
DB_HOST=mysql                    # Database host
DB_PORT=3306                     # Database port
DB_NAME=studio_db                # Database name
DB_USER=root                     # Database user
DB_PASSWORD=sachin1605           # Database password (CHANGE THIS!)
API_PORT=5000                    # API port
ASPNETCORE_ENVIRONMENT=Development
LOG_LEVEL=Information
```

### Change Port

```bash
# Edit .env
API_PORT=8000

# Restart services
docker-compose restart studioflow

# API now at http://localhost:8000
```

---

## 📊 Common Commands

```bash
# Build image
docker-compose build

# Start services
docker-compose up -d

# Stop services
docker-compose stop

# Restart services
docker-compose restart

# View logs (all services)
docker-compose logs

# View logs (specific service)
docker-compose logs studioflow
docker-compose logs mysql

# View logs (real-time)
docker-compose logs -f studioflow

# Execute command in container
docker-compose exec studioflow-api dotnet build

# Remove services
docker-compose down

# Remove services + volumes
docker-compose down -v

# Check resource usage
docker stats

# List running containers
docker ps

# List all containers
docker ps -a
```

---

## 🐛 Quick Troubleshooting

| Issue | Solution |
|-------|----------|
| **Port already in use** | Change `API_PORT` in .env |
| **Database connection error** | Check MySQL logs: `docker-compose logs mysql` |
| **API not responding** | Check API logs: `docker-compose logs studioflow` |
| **Data lost after restart** | Volumes are persistent by default |
| **Want fresh database** | Run `docker-compose down -v` then `docker-compose up -d` |

---

## 🌐 Access Points

| Service | URL | Purpose |
|---------|-----|---------|
| **API** | `http://localhost:5000` | Main application |
| **Health** | `http://localhost:5000/health` | Health check |
| **Database** | `localhost:3306` | Direct connection |
| **Swagger** | `http://localhost:5000/swagger` | API docs (if enabled) |

---

## 💾 Volumes & Persistence

```bash
# View volumes
docker volume ls

# Inspect volume
docker volume inspect studioflow_mysql_data

# Backup data
docker run --rm -v studioflow_mysql_data:/data \
  -v $(pwd):/backup \
  ubuntu tar czf /backup/backup.tar.gz -C /data .

# List backed up files
tar tzf backup.tar.gz
```

---

## 🚢 Next Steps

### For Development
1. Read: `DOCKER_DEPLOYMENT_GUIDE.md`
2. Edit: `.env` with your settings
3. Run: `docker-compose up -d`
4. Develop!

### For AWS Deployment
1. Read: `AWS_DEPLOYMENT_GUIDE.md`
2. Follow: Step-by-step instructions
3. Deploy: To AWS ECS/EC2

### For Production
1. Change: Database password
2. Update: Environment variables
3. Test: All endpoints
4. Monitor: Logs and metrics

---

## 📚 Documentation Links

- **Detailed Docker Guide**: `Docs/Deployment/DOCKER_DEPLOYMENT_GUIDE.md`
- **AWS Deployment**: `Docs/Deployment/AWS_DEPLOYMENT_GUIDE.md`
- **Environment Variables**: `.env.example`

---

## ⚙️ Advanced: Build & Push to AWS

```bash
# Set variables
export AWS_ACCOUNT_ID=123456789
export ECR_REGISTRY=$AWS_ACCOUNT_ID.dkr.ecr.us-east-1.amazonaws.com

# Authenticate with ECR
aws ecr get-login-password --region us-east-1 | \
  docker login --username AWS --password-stdin $ECR_REGISTRY

# Build image
docker build -t studioflow:latest .

# Tag for ECR
docker tag studioflow:latest $ECR_REGISTRY/studioflow:latest

# Push to ECR
docker push $ECR_REGISTRY/studioflow:latest

# Verify
aws ecr describe-images --repository-name studioflow
```

---

## ✅ Quick Verification Checklist

- [ ] Docker & Docker Compose installed
- [ ] `.env` file exists
- [ ] `docker-compose up -d` succeeded
- [ ] Both containers show "healthy" status
- [ ] `curl http://localhost:5000/health` returns 200
- [ ] Can connect to API endpoints

---

**Status:** ✅ Ready to Deploy
**Updated:** March 17, 2026

