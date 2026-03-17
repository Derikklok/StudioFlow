# 🚀 Docker & Docker Compose Deployment Guide

## Overview

This guide covers building Docker images and deploying StudioFlow using Docker Compose for:
- ✅ Local development and testing
- ✅ Local Docker testing (simulating production)
- ✅ AWS deployment preparation

---

## 📋 Prerequisites

### Local Development
- Docker Desktop installed and running
- Docker Compose (included with Docker Desktop)
- .NET 10 SDK (optional - for local development without Docker)
- MySQL Client tools (optional - for debugging)

### AWS Deployment
- AWS Account with appropriate permissions
- AWS CLI configured
- ECR (Elastic Container Registry) repository
- ECS (Elastic Container Service) cluster or EC2 instances
- RDS (Relational Database Service) MySQL instance
- CloudFormation or Terraform (recommended for IaC)

---

## 🏗️ Architecture

```
┌─────────────────────────────────────┐
│       Docker Compose Network        │
├─────────────────────────────────────┤
│                                     │
│  ┌──────────────┐  ┌────────────┐  │
│  │   MySQL DB   │  │   API App  │  │
│  │   (Port 3306)│  │ (Port 5000)│  │
│  └──────────────┘  └────────────┘  │
│        ▲                   ▲        │
│        └───────────────────┘        │
│     Internal Communication          │
│        (mysql service)              │
│                                     │
└─────────────────────────────────────┘
         ▼
    Volumes (Data Persistence)
    └─ mysql_data:/var/lib/mysql
    └─ ./logs:/app/logs
```

---

## 🔧 Environment Variables

### Configuration Files

1. **`.env`** (Development/Testing)
   - Local development settings
   - Default passwords for testing
   - Development logging

2. **`.env.example`** (Template)
   - Example configuration
   - Documentation for all variables
   - Copy and customize as needed

3. **`.env.aws`** (Production)
   - AWS-specific settings
   - Uses AWS Secrets Manager references
   - Production logging levels

### Environment Variables Reference

| Variable | Purpose | Development | Production |
|----------|---------|-------------|-----------|
| `DB_HOST` | Database host | mysql | RDS endpoint |
| `DB_PORT` | Database port | 3306 | 3306 |
| `DB_NAME` | Database name | studio_db | studio_db |
| `DB_USER` | Database user | root | admin |
| `DB_PASSWORD` | Database password | sachin1605 | AWS Secrets |
| `API_PORT` | API port | 5000 | 80/443 |
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | Development | Production |
| `LOG_LEVEL` | Logging level | Information | Warning |

---

## 🐳 Docker & Docker Compose Files

### Files Structure

```
StudioFlow/
├── Dockerfile ........................ Multi-stage build
├── docker-compose.yml ............... Local/Testing setup
├── docker-compose.aws.yml ........... AWS reference
├── .env ............................ Local environment
├── .env.example .................... Template
├── .env.aws ....................... AWS production
└── Docs/Deployment/ ............... Deployment documentation
```

### Key Files Explained

#### `Dockerfile`
- Multi-stage build (build → publish → final)
- Optimized for production
- Minimal final image size
- Security best practices

#### `docker-compose.yml`
- Local development and testing
- MySQL service with health checks
- Volume mounts for data persistence
- Custom network for service communication
- Environment variable support

#### `docker-compose.aws.yml`
- AWS deployment reference
- Uses ECR images
- Production-grade configuration
- EBS/EFS volume support

---

## 📱 Volumes & Data Persistence

### Types of Volumes

1. **Named Volume (mysql_data)**
   - Persists MySQL database data
   - Survives container restarts
   - Location: Docker volume storage

2. **Bind Mount (./logs)**
   - Local directory mounted in container
   - Persists application logs
   - Accessible from host filesystem

3. **AWS Volumes**
   - EBS volumes for persistent data
   - EFS for shared filesystem
   - CloudWatch Logs for centralized logging

### Volume Configuration

```yaml
volumes:
  mysql_data:              # Named volume
    driver: local
    
  ./logs:/app/logs         # Bind mount (relative path)
```

### Data Persistence Workflow

```
Local Machine                  Container
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
./logs ━━━━━━━ mount ━━━━━→ /app/logs
                            (write logs)
                              ↓
                        logs persist in ./logs
                        (survives restart)
```

---

## 🚀 Quick Start - Local Testing

### Step 1: Prepare Environment

```bash
# Navigate to project directory
cd ~/Desktop/Programming/ASP/StudioFlow

# Copy example environment file (optional)
cp .env.example .env

# Review and update .env if needed
# Default credentials work for local testing
```

### Step 2: Build and Start Services

```bash
# Build Docker image and start all services
docker-compose up -d

# Output: Creating studioflow-mysql ... done
#         Creating studioflow-api ... done

# Check status
docker-compose ps

# Output: NAME                    STATUS
#         studioflow-mysql        Up (healthy)
#         studioflow-api          Up (healthy)
```

### Step 3: Verify Services

```bash
# Check API is responding
curl http://localhost:5000/health

# Expected: 200 OK response

# Check database connection
docker exec studioflow-mysql mysqladmin ping -h localhost

# Expected: mysqld is alive
```

### Step 4: View Logs

```bash
# API logs
docker-compose logs studioflow

# Database logs
docker-compose logs mysql

# Follow logs in real-time
docker-compose logs -f

# Stop following (Ctrl+C)
```

### Step 5: Run Migrations (if needed)

```bash
# Inside container
docker exec studioflow-api dotnet ef database update

# Or execute in container
docker-compose exec studioflow-api dotnet ef database update
```

### Step 6: Test API Endpoints

```bash
# Create a user
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -d '{"name":"Test User","email":"test@example.com","password":"password","role":"User"}'

# Expected: 201 Created
```

### Step 7: Stop Services

```bash
# Stop all services (keeps data in volumes)
docker-compose down

# Stop and remove volumes (deletes data)
docker-compose down -v

# Output: Removing studioflow-api ... done
#         Removing studioflow-mysql ... done
#         Removing network studioflow-network
```

---

## 🐳 Docker Commands Reference

### Build & Run

```bash
# Build image
docker-compose build

# Build with no cache
docker-compose build --no-cache

# Start services
docker-compose up -d

# Start specific service
docker-compose up -d studioflow

# Stop services
docker-compose down

# Restart services
docker-compose restart
```

### Debugging

```bash
# View logs
docker-compose logs studioflow

# Real-time logs
docker-compose logs -f studioflow

# View specific lines
docker-compose logs studioflow --tail=100

# Container shell
docker-compose exec studioflow-api bash

# Run command in container
docker-compose exec studioflow-api dotnet build

# Check resource usage
docker stats

# Inspect container
docker inspect studioflow-api
```

### Volume Management

```bash
# List volumes
docker volume ls

# Inspect volume
docker volume inspect studioflow_mysql_data

# Remove volume
docker volume rm studioflow_mysql_data

# Backup volume
docker run --rm -v studioflow_mysql_data:/data \
  -v $(pwd):/backup \
  ubuntu tar czf /backup/mysql-backup.tar.gz -C /data .

# Restore volume
docker run --rm -v studioflow_mysql_data:/data \
  -v $(pwd):/backup \
  ubuntu bash -c "cd /data && tar xzf /backup/mysql-backup.tar.gz"
```

---

## ☁️ AWS Deployment

### Option 1: AWS ECS (Recommended)

#### Step 1: Push Image to ECR

```bash
# Authenticate Docker with ECR
aws ecr get-login-password --region us-east-1 | \
  docker login --username AWS --password-stdin \
  YOUR_ACCOUNT_ID.dkr.ecr.us-east-1.amazonaws.com

# Build image
docker build -t studioflow:latest .

# Tag image for ECR
docker tag studioflow:latest \
  YOUR_ACCOUNT_ID.dkr.ecr.us-east-1.amazonaws.com/studioflow:latest

# Push to ECR
docker push YOUR_ACCOUNT_ID.dkr.ecr.us-east-1.amazonaws.com/studioflow:latest
```

#### Step 2: Create ECS Task Definition

```json
{
  "family": "studioflow",
  "networkMode": "awsvpc",
  "requiresCompatibilities": ["FARGATE"],
  "cpu": "256",
  "memory": "512",
  "containerDefinitions": [{
    "name": "studioflow",
    "image": "YOUR_ACCOUNT_ID.dkr.ecr.us-east-1.amazonaws.com/studioflow:latest",
    "portMappings": [{"containerPort": 80, "protocol": "tcp"}],
    "environment": [
      {"name": "ASPNETCORE_ENVIRONMENT", "value": "Production"},
      {"name": "DB_HOST", "value": "your-rds-endpoint.rds.amazonaws.com"}
    ],
    "secrets": [
      {"name": "DB_PASSWORD", "valueFrom": "arn:aws:secretsmanager:..."}
    ],
    "logConfiguration": {
      "logDriver": "awslogs",
      "options": {
        "awslogs-group": "/ecs/studioflow",
        "awslogs-region": "us-east-1",
        "awslogs-stream-prefix": "ecs"
      }
    }
  }]
}
```

#### Step 3: Create ECS Service

```bash
aws ecs create-service \
  --cluster studioflow-cluster \
  --service-name studioflow-service \
  --task-definition studioflow:1 \
  --desired-count 2 \
  --launch-type FARGATE \
  --network-configuration "awsvpcConfiguration={subnets=[subnet-xxx],securityGroups=[sg-xxx]}" \
  --load-balancers targetGroupArn=arn:aws:elasticloadbalancing:...,containerName=studioflow,containerPort=80
```

### Option 2: AWS EC2 with Docker Compose

#### Step 1: Launch EC2 Instance

```bash
# Use Ubuntu 22.04 LTS AMI
# Select instance type (t2.medium or larger recommended)
# Configure security groups (allow 80, 443, 3306)
```

#### Step 2: Connect and Install Docker

```bash
# SSH into instance
ssh -i your-key.pem ubuntu@your-ec2-ip

# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo usermod -aG docker ubuntu

# Install Docker Compose
sudo curl -L \
  "https://github.com/docker/compose/releases/download/v2.20.0/docker-compose-$(uname -s)-$(uname -m)" \
  -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Verify installation
docker --version
docker-compose --version
```

#### Step 3: Deploy Application

```bash
# Clone repository
git clone your-repo-url
cd StudioFlow

# Copy AWS environment file
cp .env.aws .env

# Update .env with AWS RDS endpoint and credentials
# Use AWS Secrets Manager for passwords

# Start services
docker-compose -f docker-compose.aws.yml up -d

# Verify
docker-compose ps
```

---

## 🔒 Security Best Practices

### For Production

1. **Change Default Passwords**
   ```bash
   # Update DB_PASSWORD in .env
   # Use strong, random passwords (30+ characters)
   ```

2. **Use AWS Secrets Manager**
   ```bash
   # Store sensitive values in AWS Secrets Manager
   # Reference in environment: arn:aws:secretsmanager:region:account:secret:name
   ```

3. **Network Security**
   ```bash
   # Restrict database access to app only
   # Use security groups to limit access
   # Never expose database port to internet
   ```

4. **HTTPS/TLS**
   ```bash
   # Use AWS ALB/NLB with HTTPS
   # Configure SSL certificates (ACM)
   # Redirect HTTP to HTTPS
   ```

5. **Environment Variables**
   - Never commit .env file to git
   - Use .gitignore:
     ```
     .env
     .env.local
     .env.*.local
     logs/
     ```

6. **Image Security**
   - Use specific base image versions (not latest)
   - Scan images for vulnerabilities
   - Use minimal base images

---

## 📊 Monitoring & Logging

### Local Monitoring

```bash
# Resource usage
docker stats

# Container logs
docker-compose logs -f

# Database logs
docker-compose logs -f mysql
```

### AWS Monitoring

```bash
# CloudWatch Logs
aws logs tail /ecs/studioflow --follow

# CloudWatch Metrics
aws cloudwatch get-metric-statistics \
  --namespace AWS/ECS \
  --metric-name CPUUtilization \
  --dimensions Name=ServiceName,Value=studioflow-service \
  --statistics Average \
  --start-time 2024-01-01T00:00:00Z \
  --end-time 2024-01-02T00:00:00Z \
  --period 300
```

---

## 🆘 Troubleshooting

### Database Connection Issues

```bash
# Check database is running
docker-compose ps mysql

# Check database logs
docker-compose logs mysql

# Test connection
docker exec studioflow-api dotnet-user-secrets set \
  "ConnectionStrings:DefaultConnection" \
  "server=mysql;database=studio_db;user=root;password=sachin1605"

# Verify from container
docker-compose exec studioflow-api bash
mysql -h mysql -u root -p -e "SELECT 1"
```

### API Not Responding

```bash
# Check container status
docker-compose ps studioflow

# View logs
docker-compose logs studioflow --tail=50

# Check health
curl -i http://localhost:5000/health

# Restart
docker-compose restart studioflow
```

### Volume Issues

```bash
# List volumes
docker volume ls | grep studioflow

# Check volume data
docker run --rm -v studioflow_mysql_data:/data ubuntu ls -la /data

# Clear volume
docker volume rm studioflow_mysql_data
docker-compose up -d  # Recreate volume
```

### Port Already in Use

```bash
# Find process using port
sudo lsof -i :5000
sudo lsof -i :3306

# Kill process or change port in .env
# Restart services
docker-compose down
docker-compose up -d
```

---

## 📚 Additional Resources

- Docker Documentation: https://docs.docker.com
- Docker Compose: https://docs.docker.com/compose
- AWS ECS: https://docs.aws.amazon.com/ecs
- AWS Fargate: https://docs.aws.amazon.com/AmazonECS/latest/developerguide/launch_types.html
- Best Practices: https://docs.docker.com/develop/dev-best-practices

---

## ✅ Deployment Checklist

### Pre-Deployment
- [ ] Update `.env` with actual credentials
- [ ] Change default database password
- [ ] Review security groups
- [ ] Configure backup strategy
- [ ] Test locally with `docker-compose up`

### AWS Deployment
- [ ] Create RDS instance
- [ ] Create ECR repository
- [ ] Build and push Docker image
- [ ] Create ECS cluster
- [ ] Create task definition
- [ ] Create load balancer
- [ ] Configure health checks
- [ ] Set up CloudWatch logs

### Post-Deployment
- [ ] Verify API endpoints responding
- [ ] Check database connectivity
- [ ] Monitor logs for errors
- [ ] Load test application
- [ ] Configure auto-scaling
- [ ] Set up monitoring/alerting

---

**Status:** ✅ Production Ready
**Last Updated:** March 17, 2026

