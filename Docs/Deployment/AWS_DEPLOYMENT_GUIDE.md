# ☁️ AWS Deployment Guide - StudioFlow

## Complete AWS Deployment Strategy

This guide provides step-by-step instructions for deploying StudioFlow to AWS using ECS Fargate, RDS, and other AWS services.

---

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                     AWS Account                         │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  ┌─────────────────────────────────────────────────┐   │
│  │         Elastic Load Balancer (ALB)             │   │
│  │  - HTTPS listener (443)                         │   │
│  │  - HTTP redirect (80 → 443)                     │   │
│  └────────────────────┬────────────────────────────┘   │
│                       │                                 │
│  ┌────────────────────▼────────────────────────────┐   │
│  │      ECS Cluster (Fargate)                      │   │
│  │  ┌──────────────┐  ┌──────────────┐             │   │
│  │  │  StudioFlow  │  │  StudioFlow  │ (Auto-scale)│   │
│  │  │   Container  │  │   Container  │             │   │
│  │  └──────────────┘  └──────────────┘             │   │
│  └────────────────────┬────────────────────────────┘   │
│                       │                                 │
│  ┌────────────────────▼────────────────────────────┐   │
│  │    RDS MySQL (Multi-AZ)                         │   │
│  │  - Automated backups                            │   │
│  │  - Read replicas (optional)                     │   │
│  └─────────────────────────────────────────────────┘   │
│                                                         │
│  ┌─────────────────────────────────────────────────┐   │
│  │    CloudWatch & Logging                         │   │
│  │  - Container logs                               │   │
│  │  - Application metrics                          │   │
│  │  - Alarms & notifications                       │   │
│  └─────────────────────────────────────────────────┘   │
│                                                         │
│  ┌─────────────────────────────────────────────────┐   │
│  │    Secrets Manager                              │   │
│  │  - Database credentials                         │   │
│  │  - API keys (future)                            │   │
│  └─────────────────────────────────────────────────┘   │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

---

## 📋 Prerequisites

### AWS Account Setup
- [ ] AWS Account created
- [ ] Appropriate IAM permissions
- [ ] AWS CLI v2 installed and configured
- [ ] VPC with public/private subnets created
- [ ] NAT Gateway configured (for private subnet egress)

### Required AWS Services
- [ ] ECR (Elastic Container Registry)
- [ ] ECS (Elastic Container Service)
- [ ] RDS (MySQL 8.0)
- [ ] ALB (Application Load Balancer)
- [ ] Secrets Manager
- [ ] CloudWatch Logs
- [ ] IAM Roles & Policies

---

## 🚀 Step-by-Step Deployment

### Phase 1: Prepare Docker Image

#### Step 1.1: Build Local Image

```bash
# Navigate to project directory
cd ~/Desktop/Programming/ASP/StudioFlow

# Build image with tag
docker build -t studioflow:latest .

# Verify build
docker images | grep studioflow

# Expected: studioflow  latest  abc123...  10 seconds ago  ...
```

#### Step 1.2: Test Image Locally

```bash
# Run container locally
docker run -d \
  -e "ASPNETCORE_ENVIRONMENT=Production" \
  -e "ConnectionStrings__DefaultConnection=server=host.docker.internal;..." \
  -p 5000:80 \
  studioflow:latest

# Test
curl http://localhost:5000/health

# Stop container
docker stop <container-id>
```

### Phase 2: Push to AWS ECR

#### Step 2.1: Create ECR Repository

```bash
# Create repository
aws ecr create-repository \
  --repository-name studioflow \
  --region us-east-1 \
  --image-scan-on-push \
  --encryption-configuration encryptionType=AES256

# Response: {
#   "repository": {
#     "repositoryUri": "YOUR_ACCOUNT_ID.dkr.ecr.us-east-1.amazonaws.com/studioflow",
#     ...
#   }
# }

# Save repository URI
export ECR_URI="YOUR_ACCOUNT_ID.dkr.ecr.us-east-1.amazonaws.com/studioflow"
```

#### Step 2.2: Authenticate Docker with ECR

```bash
# Get authorization token
aws ecr get-login-password --region us-east-1 | \
  docker login --username AWS --password-stdin ${ECR_URI}

# Expected: Login Succeeded
```

#### Step 2.3: Tag and Push Image

```bash
# Tag image for ECR
docker tag studioflow:latest ${ECR_URI}:latest
docker tag studioflow:latest ${ECR_URI}:v1.0.0

# Push images
docker push ${ECR_URI}:latest
docker push ${ECR_URI}:v1.0.0

# Verify push
aws ecr describe-images \
  --repository-name studioflow \
  --region us-east-1
```

### Phase 3: Create RDS Database

#### Step 3.1: Create RDS Instance

```bash
# Create MySQL instance
aws rds create-db-instance \
  --db-instance-identifier studioflow-db \
  --db-instance-class db.t3.micro \
  --engine mysql \
  --engine-version 8.0.35 \
  --allocated-storage 20 \
  --storage-type gp3 \
  --master-username admin \
  --master-user-password "$(openssl rand -base64 12)" \
  --vpc-security-group-ids sg-xxxxx \
  --db-subnet-group-name default \
  --backup-retention-period 30 \
  --multi-az \
  --enable-cloudwatch-logs-exports error general slowquery \
  --region us-east-1

# Note: Save the password securely
```

#### Step 3.2: Create Database and User

```bash
# Wait for instance to be available (~5-10 minutes)
aws rds wait db-instance-available \
  --db-instance-identifier studioflow-db \
  --region us-east-1

# Connect to database
mysql -h studioflow-db.xxxxxxx.us-east-1.rds.amazonaws.com \
  -u admin -p

# Create database
CREATE DATABASE studio_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

# Create application user
CREATE USER 'studioflow'@'%' IDENTIFIED BY 'strong_password_here';
GRANT ALL PRIVILEGES ON studio_db.* TO 'studioflow'@'%';
FLUSH PRIVILEGES;

# Verify
SHOW DATABASES;
SHOW USERS;
```

#### Step 3.3: Store Credentials in Secrets Manager

```bash
# Create secret for database password
aws secretsmanager create-secret \
  --name studioflow/db-password \
  --description "StudioFlow RDS password" \
  --secret-string "strong_password_here" \
  --region us-east-1

# Get secret ARN
aws secretsmanager describe-secret \
  --secret-id studioflow/db-password \
  --region us-east-1

# Save the ARN for task definition
export DB_PASSWORD_ARN="arn:aws:secretsmanager:..."
```

### Phase 4: Create ECS Infrastructure

#### Step 4.1: Create CloudWatch Log Group

```bash
# Create log group
aws logs create-log-group \
  --log-group-name /ecs/studioflow \
  --region us-east-1

# Set retention
aws logs put-retention-policy \
  --log-group-name /ecs/studioflow \
  --retention-in-days 30 \
  --region us-east-1
```

#### Step 4.2: Create IAM Task Execution Role

```bash
# Create trust policy
cat > trust-policy.json << EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Principal": {
        "Service": "ecs-tasks.amazonaws.com"
      },
      "Action": "sts:AssumeRole"
    }
  ]
}
EOF

# Create role
aws iam create-role \
  --role-name studioflow-task-execution-role \
  --assume-role-policy-document file://trust-policy.json

# Attach policy
aws iam attach-role-policy \
  --role-name studioflow-task-execution-role \
  --policy-arn arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy

# Create inline policy for secrets access
cat > secrets-policy.json << EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "secretsmanager:GetSecretValue"
      ],
      "Resource": "arn:aws:secretsmanager:us-east-1:*:secret:studioflow/*"
    },
    {
      "Effect": "Allow",
      "Action": [
        "kms:Decrypt"
      ],
      "Resource": "arn:aws:kms:us-east-1:*:key/*"
    }
  ]
}
EOF

aws iam put-role-policy \
  --role-name studioflow-task-execution-role \
  --policy-name studioflow-secrets-policy \
  --policy-document file://secrets-policy.json
```

#### Step 4.3: Create Task Definition

```bash
# Create task definition JSON
cat > task-definition.json << 'EOF'
{
  "family": "studioflow",
  "networkMode": "awsvpc",
  "requiresCompatibilities": ["FARGATE"],
  "cpu": "256",
  "memory": "512",
  "containerDefinitions": [
    {
      "name": "studioflow",
      "image": "YOUR_ACCOUNT_ID.dkr.ecr.us-east-1.amazonaws.com/studioflow:latest",
      "portMappings": [
        {
          "containerPort": 80,
          "hostPort": 80,
          "protocol": "tcp"
        }
      ],
      "environment": [
        {
          "name": "ASPNETCORE_ENVIRONMENT",
          "value": "Production"
        },
        {
          "name": "ASPNETCORE_URLS",
          "value": "http://+:80"
        },
        {
          "name": "Logging__LogLevel__Default",
          "value": "Warning"
        },
        {
          "name": "Logging__LogLevel__Microsoft__AspNetCore",
          "value": "Warning"
        },
        {
          "name": "DB_HOST",
          "value": "studioflow-db.xxxxxxx.us-east-1.rds.amazonaws.com"
        },
        {
          "name": "DB_PORT",
          "value": "3306"
        },
        {
          "name": "DB_NAME",
          "value": "studio_db"
        },
        {
          "name": "DB_USER",
          "value": "studioflow"
        }
      ],
      "secrets": [
        {
          "name": "DB_PASSWORD",
          "valueFrom": "arn:aws:secretsmanager:us-east-1:ACCOUNT_ID:secret:studioflow/db-password:password::"
        }
      ],
      "logConfiguration": {
        "logDriver": "awslogs",
        "options": {
          "awslogs-group": "/ecs/studioflow",
          "awslogs-region": "us-east-1",
          "awslogs-stream-prefix": "ecs"
        }
      }
    }
  ],
  "executionRoleArn": "arn:aws:iam::ACCOUNT_ID:role/studioflow-task-execution-role"
}
EOF

# Register task definition
aws ecs register-task-definition \
  --cli-input-json file://task-definition.json \
  --region us-east-1
```

#### Step 4.4: Create ECS Cluster

```bash
# Create cluster
aws ecs create-cluster \
  --cluster-name studioflow \
  --cluster-settings name=containerInsights,value=enabled \
  --region us-east-1
```

#### Step 4.5: Create Load Balancer

```bash
# Create target group
aws elbv2 create-target-group \
  --name studioflow-tg \
  --protocol HTTP \
  --port 80 \
  --vpc-id vpc-xxxxx \
  --health-check-enabled \
  --health-check-path /health \
  --health-check-interval-seconds 30 \
  --health-check-timeout-seconds 5 \
  --healthy-threshold-count 2 \
  --unhealthy-threshold-count 3 \
  --region us-east-1

# Create load balancer
aws elbv2 create-load-balancer \
  --name studioflow-alb \
  --subnets subnet-xxxxx subnet-yyyyy \
  --security-groups sg-xxxxx \
  --scheme internet-facing \
  --type application \
  --ip-address-type ipv4 \
  --region us-east-1

# Create listener
aws elbv2 create-listener \
  --load-balancer-arn arn:aws:elasticloadbalancing:... \
  --protocol HTTP \
  --port 80 \
  --default-actions Type=forward,TargetGroupArn=arn:aws:elasticloadbalancing:... \
  --region us-east-1
```

#### Step 4.6: Create ECS Service

```bash
# Create service
aws ecs create-service \
  --cluster studioflow \
  --service-name studioflow-service \
  --task-definition studioflow:1 \
  --desired-count 2 \
  --launch-type FARGATE \
  --network-configuration "awsvpcConfiguration={
    subnets=[subnet-xxxxx,subnet-yyyyy],
    securityGroups=[sg-xxxxx],
    assignPublicIp=DISABLED
  }" \
  --load-balancers "targetGroupArn=arn:aws:elasticloadbalancing:...,containerName=studioflow,containerPort=80" \
  --deployment-configuration "maximumPercent=200,minimumHealthyPercent=100" \
  --enable-ecs-managed-tags \
  --propagate-tags SERVICE \
  --region us-east-1

# Verify service created
aws ecs describe-services \
  --cluster studioflow \
  --services studioflow-service \
  --region us-east-1
```

### Phase 5: Configure Auto-Scaling

#### Step 5.1: Create Auto-Scaling Target

```bash
# Register scalable target
aws application-autoscaling register-scalable-target \
  --service-namespace ecs \
  --resource-id service/studioflow/studioflow-service \
  --scalable-dimension ecs:service:DesiredCount \
  --min-capacity 2 \
  --max-capacity 10 \
  --region us-east-1
```

#### Step 5.2: Create Scaling Policy

```bash
# CPU-based scaling
aws application-autoscaling put-scaling-policy \
  --policy-name studioflow-cpu-scaling \
  --service-namespace ecs \
  --resource-id service/studioflow/studioflow-service \
  --scalable-dimension ecs:service:DesiredCount \
  --policy-type TargetTrackingScaling \
  --target-tracking-scaling-policy-configuration "
    TargetValue=70.0,
    PredefinedMetricSpecification={
      PredefinedMetricType=ECSServiceAverageCPUUtilization
    },
    ScaleOutCooldown=300,
    ScaleInCooldown=300
  " \
  --region us-east-1

# Memory-based scaling
aws application-autoscaling put-scaling-policy \
  --policy-name studioflow-memory-scaling \
  --service-namespace ecs \
  --resource-id service/studioflow/studioflow-service \
  --scalable-dimension ecs:service:DesiredCount \
  --policy-type TargetTrackingScaling \
  --target-tracking-scaling-policy-configuration "
    TargetValue=80.0,
    PredefinedMetricSpecification={
      PredefinedMetricType=ECSServiceAverageMemoryUtilization
    },
    ScaleOutCooldown=300,
    ScaleInCooldown=300
  " \
  --region us-east-1
```

---

## 📊 Post-Deployment Verification

### Health Checks

```bash
# Check service status
aws ecs describe-services \
  --cluster studioflow \
  --services studioflow-service \
  --query 'services[0].[desiredCount,runningCount,serviceName]' \
  --region us-east-1

# Expected: [2, 2, studioflow-service]

# Check task status
aws ecs list-tasks \
  --cluster studioflow \
  --service-name studioflow-service \
  --region us-east-1

# Get load balancer DNS
aws elbv2 describe-load-balancers \
  --names studioflow-alb \
  --query 'LoadBalancers[0].DNSName' \
  --region us-east-1

# Test API endpoint
curl http://studioflow-alb-xxxxx.us-east-1.elb.amazonaws.com/health
```

### Monitoring

```bash
# View container logs
aws logs tail /ecs/studioflow --follow

# CPU metrics
aws cloudwatch get-metric-statistics \
  --namespace AWS/ECS \
  --metric-name CPUUtilization \
  --dimensions Name=ServiceName,Value=studioflow-service Name=ClusterName,Value=studioflow \
  --start-time 2024-01-01T00:00:00Z \
  --end-time 2024-01-02T00:00:00Z \
  --period 3600 \
  --statistics Average,Maximum

# Memory metrics
aws cloudwatch get-metric-statistics \
  --namespace AWS/ECS \
  --metric-name MemoryUtilization \
  --dimensions Name=ServiceName,Value=studioflow-service Name=ClusterName,Value=studioflow \
  --start-time 2024-01-01T00:00:00Z \
  --end-time 2024-01-02T00:00:00Z \
  --period 3600 \
  --statistics Average,Maximum
```

---

## 🔄 CI/CD Pipeline Integration

### GitHub Actions Example

```yaml
name: Deploy to AWS ECS

on:
  push:
    branches: [ main ]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      
      - name: Configure AWS credentials
        uses: aws-actions/configure-aws-credentials@v1
        with:
          aws-access-key-id: ${{ secrets.AWS_ACCESS_KEY_ID }}
          aws-secret-access-key: ${{ secrets.AWS_SECRET_ACCESS_KEY }}
          aws-region: us-east-1
      
      - name: Login to Amazon ECR
        run: |
          aws ecr get-login-password --region us-east-1 | \
            docker login --username AWS --password-stdin \
            ${{ secrets.AWS_ACCOUNT_ID }}.dkr.ecr.us-east-1.amazonaws.com
      
      - name: Build, tag, and push image to ECR
        run: |
          docker build -t studioflow:latest .
          docker tag studioflow:latest \
            ${{ secrets.AWS_ACCOUNT_ID }}.dkr.ecr.us-east-1.amazonaws.com/studioflow:${{ github.sha }}
          docker push \
            ${{ secrets.AWS_ACCOUNT_ID }}.dkr.ecr.us-east-1.amazonaws.com/studioflow:${{ github.sha }}
      
      - name: Update ECS service
        run: |
          aws ecs update-service \
            --cluster studioflow \
            --service studioflow-service \
            --force-new-deployment \
            --region us-east-1
```

---

## 📚 Troubleshooting

### Task Won't Start

```bash
# Check task logs
aws ecs describe-tasks \
  --cluster studioflow \
  --tasks arn:aws:ecs:... \
  --query 'tasks[0].[lastStatus,stoppedReason]' \
  --region us-east-1

# View CloudWatch logs
aws logs tail /ecs/studioflow --follow

# Common issues:
# 1. Database connection: Check RDS security group
# 2. Memory/CPU: Increase task definition limits
# 3. Image pull: Verify ECR access and image exists
```

### Database Connection Failed

```bash
# Test RDS connectivity
mysql -h studioflow-db.xxxxxxx.us-east-1.rds.amazonaws.com \
  -u studioflow -p studio_db \
  -e "SELECT 1"

# Check RDS security group
aws ec2 describe-security-groups \
  --group-ids sg-xxxxx \
  --query 'SecurityGroups[0].IpPermissions' \
  --region us-east-1

# Update security group if needed
aws ec2 authorize-security-group-ingress \
  --group-id sg-xxxxx \
  --protocol tcp \
  --port 3306 \
  --source-group sg-yyyyy \
  --region us-east-1
```

---

## 📋 Maintenance Tasks

### Regular Backups

```bash
# RDS automatic backups are enabled
# Manual snapshot
aws rds create-db-snapshot \
  --db-instance-identifier studioflow-db \
  --db-snapshot-identifier studioflow-db-backup-$(date +%Y%m%d) \
  --region us-east-1
```

### Update Task Definition

```bash
# Update image in task definition
aws ecs update-service \
  --cluster studioflow \
  --service studioflow-service \
  --force-new-deployment \
  --region us-east-1
```

### Scale Service

```bash
# Update desired count
aws ecs update-service \
  --cluster studioflow \
  --service studioflow-service \
  --desired-count 5 \
  --region us-east-1
```

---

## ✅ Deployment Checklist

- [ ] Docker image built and tested
- [ ] ECR repository created
- [ ] Image pushed to ECR
- [ ] RDS instance created
- [ ] Database and user created
- [ ] Secrets stored in Secrets Manager
- [ ] CloudWatch log group created
- [ ] IAM roles and policies configured
- [ ] Task definition registered
- [ ] ECS cluster created
- [ ] Load balancer configured
- [ ] ECS service created
- [ ] Auto-scaling configured
- [ ] Health checks passing
- [ ] Application accessible via ALB
- [ ] Monitoring alerts configured
- [ ] Backup strategy implemented

---

**Status:** ✅ Ready for Production
**Last Updated:** March 17, 2026

