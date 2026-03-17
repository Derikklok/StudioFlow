# ✅ Complete Deployment Checklist

Comprehensive checklist for deploying StudioFlow using Docker and AWS.

---

## 📋 Pre-Deployment (Local Testing)

### Environment Setup
- [ ] Docker Desktop installed and running
- [ ] Docker Compose installed (`docker-compose --version`)
- [ ] Project repository cloned/downloaded
- [ ] All source code files present
- [ ] `.env` file copied from `.env.example`

### File Validation
- [ ] `Dockerfile` exists and is valid
- [ ] `docker-compose.yml` exists and is valid
- [ ] `.env` file exists with correct values
- [ ] `.gitignore` includes `.env`
- [ ] All source code files in place

### Configuration Review
- [ ] Database credentials reviewed
- [ ] API port not in use locally
- [ ] Environment set to Development
- [ ] Log level appropriate (Information)
- [ ] Volume paths correct

---

## 🏗️ Local Docker Setup

### Build Phase
- [ ] Navigate to project directory
- [ ] Run `docker-compose build`
- [ ] Build completes without errors
- [ ] Image appears in `docker images`
- [ ] Image size reasonable (~300-500MB)

### Container Launch
- [ ] Run `docker-compose up -d`
- [ ] All services start successfully
- [ ] `docker-compose ps` shows all "healthy"
- [ ] MySQL logs show "ready for connections"
- [ ] API logs show "application started"

### Health Verification
- [ ] API responds to health check: `curl http://localhost:5000/health`
- [ ] Database responds: `docker exec studioflow-mysql mysqladmin ping`
- [ ] No error messages in logs
- [ ] Container resource usage reasonable

### API Testing
- [ ] Test GET endpoint works
- [ ] Test POST endpoint works
- [ ] Test error handling (invalid request)
- [ ] Database queries execute successfully
- [ ] Response formats correct (JSON)

### Cleanup & Verification
- [ ] Stop containers: `docker-compose down`
- [ ] Data persists in volumes
- [ ] Restart containers: `docker-compose up -d`
- [ ] Data still present (volumes working)
- [ ] All tests pass again

---

## ☁️ AWS Pre-Deployment

### AWS Account Setup
- [ ] AWS account created and active
- [ ] IAM user with appropriate permissions
- [ ] AWS CLI installed and configured
- [ ] AWS CLI credentials validated: `aws sts get-caller-identity`
- [ ] VPC created with public/private subnets
- [ ] NAT Gateway configured (for private egress)
- [ ] Internet Gateway attached to VPC

### IAM & Access
- [ ] IAM role for ECR access created
- [ ] IAM role for ECS task execution created
- [ ] IAM role for ECS task permissions created
- [ ] Trust policies configured correctly
- [ ] Permissions tested

### Networking
- [ ] VPC created
- [ ] Public subnets created (2+ for ALB)
- [ ] Private subnets created (2+ for tasks)
- [ ] Route tables configured
- [ ] Security groups defined:
  - [ ] ALB security group (80, 443)
  - [ ] API security group (ALB source)
  - [ ] RDS security group (API source only)
  - [ ] ECS tasks security group

---

## 🐳 AWS ECR & Image Push

### ECR Repository
- [ ] AWS ECR repository created: `studioflow`
- [ ] Repository settings verified
- [ ] Image scanning enabled
- [ ] Repository URI saved: `{ACCOUNT_ID}.dkr.ecr.{REGION}.amazonaws.com/studioflow`

### Image Build & Push
- [ ] Docker image built locally
- [ ] Image tested locally
- [ ] AWS credentials configured
- [ ] Docker authenticated with ECR: `aws ecr get-login-password`
- [ ] Image tagged with ECR URI
- [ ] Image pushed to ECR
- [ ] Image appears in ECR console
- [ ] Image scan completes (no critical vulnerabilities)

### Image Verification
- [ ] Image layers correct size
- [ ] Image digest recorded
- [ ] Image tags applied (latest, v1.0.0, etc)
- [ ] `docker pull` works from ECR

---

## 🗄️ AWS RDS Database Setup

### RDS Instance Creation
- [ ] RDS instance created
  - [ ] Engine: MySQL 8.0.35
  - [ ] Class: db.t3.micro or larger
  - [ ] Storage: 20GB gp3 minimum
  - [ ] VPC: Correct VPC selected
  - [ ] Subnet group: Created
  - [ ] Security group: Restrictive
  - [ ] Multi-AZ: Enabled (production)
  - [ ] Backups: 30 day retention
- [ ] Instance status "available"
- [ ] Endpoint recorded: `studioflow-db.xxxxx.us-east-1.rds.amazonaws.com`

### Database & User Creation
- [ ] Connected to RDS instance
- [ ] Database created: `studio_db`
- [ ] Application user created: `studioflow`
- [ ] User permissions granted (only needed databases)
- [ ] Connection tested from local machine
- [ ] Connection tested from EC2/bastion

### Backup Configuration
- [ ] Automated backups enabled
- [ ] Backup retention period: 30 days
- [ ] Backup window configured
- [ ] Manual snapshot created
- [ ] Snapshot restoration tested

### Monitoring Setup
- [ ] CloudWatch detailed monitoring enabled
- [ ] Event subscriptions configured
- [ ] Alarms created:
  - [ ] CPU utilization high
  - [ ] Storage space low
  - [ ] Connection failures
  - [ ] Replication lag (if read replicas)

---

## 📦 AWS Secrets Management

### Secrets Creation
- [ ] Secrets Manager secret created: `studioflow/db-password`
- [ ] Secret value: secure password
- [ ] Encryption key: AWS managed or CMK
- [ ] Rotation policy: 90 days (recommended)
- [ ] Access policy: Restrict to ECS task role
- [ ] Secret ARN recorded

### Secret Integration
- [ ] Task definition references secret
- [ ] Secret reference format correct:
  ```
  arn:aws:secretsmanager:region:account:secret:studioflow/db-password:password::
  ```
- [ ] IAM policy allows `secretsmanager:GetSecretValue`
- [ ] KMS policy allows decryption
- [ ] Test access: `aws secretsmanager get-secret-value`

---

## 🔧 AWS ECS Configuration

### CloudWatch Logs
- [ ] Log group created: `/ecs/studioflow`
- [ ] Retention policy: 30 days
- [ ] Log stream naming convention defined
- [ ] Logs accessible in CloudWatch console

### Task Definition
- [ ] Task definition created: `studioflow`
- [ ] Task size: CPU 256, Memory 512 (minimum)
- [ ] Container definition correct
- [ ] Image URI correct (ECR repository)
- [ ] Port mappings correct (80:80)
- [ ] Environment variables set
- [ ] Secrets references configured
- [ ] Log configuration correct (awslogs driver)
- [ ] Task role and execution role assigned
- [ ] Task definition revision recorded

### ECS Cluster
- [ ] Cluster created: `studioflow`
- [ ] Cluster settings:
  - [ ] Container Insights: Enabled
  - [ ] VPC: Correct VPC
  - [ ] Subnets: Private subnets for tasks
- [ ] Security group: API security group

### ECS Service
- [ ] Service created: `studioflow-service`
- [ ] Cluster: Correct cluster
- [ ] Task definition: Correct, latest revision
- [ ] Launch type: FARGATE
- [ ] Platform version: LATEST or specific
- [ ] Desired count: 2 (minimum for HA)
- [ ] Network configuration:
  - [ ] VPC: Correct
  - [ ] Subnets: Private (2+)
  - [ ] Security group: API group
  - [ ] Public IP: DISABLED (use ALB)
- [ ] Load balancer: Configured
  - [ ] Type: Application Load Balancer
  - [ ] Target group: Correct
  - [ ] Container name: studioflow
  - [ ] Container port: 80
- [ ] Deployment configuration:
  - [ ] Maximum percent: 200
  - [ ] Minimum healthy percent: 100
- [ ] Service deployable

---

## 🌐 Load Balancing

### Application Load Balancer
- [ ] ALB created: `studioflow-alb`
- [ ] Scheme: internet-facing
- [ ] VPC: Correct
- [ ] Subnets: Public (2+)
- [ ] Security group: ALB group (80, 443)
- [ ] IP address type: ipv4

### Target Group
- [ ] Target group created: `studioflow-tg`
- [ ] Protocol: HTTP
- [ ] Port: 80
- [ ] VPC: Correct
- [ ] Health check:
  - [ ] Path: `/health`
  - [ ] Interval: 30 seconds
  - [ ] Timeout: 5 seconds
  - [ ] Healthy threshold: 2
  - [ ] Unhealthy threshold: 3
- [ ] Targets: ECS service registered

### Listeners
- [ ] HTTP listener (80): Forwards to target group
- [ ] HTTPS listener (443): Configured (if SSL)
  - [ ] Certificate: ACM or uploaded
  - [ ] Policy: Secure (TLS 1.2+)
  - [ ] HTTP redirect: 80 → 443
- [ ] Listener tested

### DNS
- [ ] Route 53 record created (if custom domain)
- [ ] CNAME/A record points to ALB
- [ ] DNS propagated globally
- [ ] Domain verified

---

## ⚙️ Auto-Scaling

### Scalable Target
- [ ] Scalable target registered
- [ ] Service: ECS
- [ ] Resource ID: service/cluster/service-name
- [ ] Min capacity: 2
- [ ] Max capacity: 10

### Scaling Policies
- [ ] CPU scaling policy:
  - [ ] Target utilization: 70%
  - [ ] Scale-out cooldown: 300s
  - [ ] Scale-in cooldown: 300s
  - [ ] Metric: ECSServiceAverageCPUUtilization
  - [ ] Policy active
- [ ] Memory scaling policy:
  - [ ] Target utilization: 80%
  - [ ] Scale-out cooldown: 300s
  - [ ] Scale-in cooldown: 300s
  - [ ] Metric: ECSServiceAverageMemoryUtilization
  - [ ] Policy active

### Testing Auto-Scaling
- [ ] Generate load on API
- [ ] Monitor task count increase
- [ ] Verify tasks scale down after load decreases
- [ ] Check CloudWatch metrics

---

## 📊 Monitoring & Alarms

### CloudWatch Metrics
- [ ] Container metrics visible:
  - [ ] CPU Utilization
  - [ ] Memory Utilization
  - [ ] Network In/Out
- [ ] Service metrics visible:
  - [ ] Desired count
  - [ ] Running count
  - [ ] Pending count
- [ ] Application metrics visible (if custom)

### Alarms Created
- [ ] High CPU utilization (>80%)
- [ ] High memory usage (>90%)
- [ ] Service unhealthy tasks (>0)
- [ ] Target unhealthy count (>0)
- [ ] Deployment failures
- [ ] ALB unhealthy targets
- [ ] RDS high connections
- [ ] RDS low storage

### Notifications
- [ ] SNS topic created: `studioflow-alarms`
- [ ] Email notifications subscribed
- [ ] Slack integration (optional):
  - [ ] Slack channel created
  - [ ] Webhook configured
  - [ ] Alarms forward to Slack
- [ ] Test alarm triggered and received

---

## 🔐 Security Review

### Credentials & Secrets
- [ ] Database password changed from default
- [ ] Password stored in Secrets Manager
- [ ] No credentials in source code
- [ ] No credentials in container images
- [ ] No credentials in logs
- [ ] API keys stored in Secrets Manager (future)

### Network Security
- [ ] ALB security group restrictive (443 only)
- [ ] API security group restrictive (ALB source)
- [ ] RDS security group restrictive (API source)
- [ ] Database not publicly accessible
- [ ] NAT Gateway used for outbound

### IAM Security
- [ ] Task execution role principle of least privilege
- [ ] Task role permissions limited to needed resources
- [ ] No * in resource policies
- [ ] Cross-account access prevented
- [ ] IAM user access keys rotated

### SSL/TLS (if implemented)
- [ ] HTTPS enabled on ALB
- [ ] Certificate valid and current
- [ ] HTTP redirects to HTTPS
- [ ] TLS version 1.2+ enforced
- [ ] Strong cipher suites

### Container Security
- [ ] Images scanned for vulnerabilities
- [ ] Base image from trusted registry
- [ ] No default credentials in image
- [ ] Non-root user (if applicable)
- [ ] Read-only filesystem (if applicable)

---

## ✔️ Post-Deployment Verification

### Service Health
- [ ] All ECS tasks running: `aws ecs list-tasks`
- [ ] All tasks in healthy state
- [ ] Target group healthy targets: 2+
- [ ] ALB shows healthy targets
- [ ] No stopped or failed tasks

### API Connectivity
- [ ] ALB DNS name accessible
- [ ] Health endpoint responds: `curl http://alb-dns/health`
- [ ] API endpoints respond correctly
- [ ] Error handling working properly
- [ ] CORS headers correct (if needed)

### Database Connectivity
- [ ] API can connect to RDS
- [ ] Database queries execute
- [ ] Connection pool working
- [ ] No connection errors in logs

### Load & Auto-Scaling
- [ ] Generate test traffic to API
- [ ] CPU metrics increase in CloudWatch
- [ ] Tasks scale up automatically
- [ ] Tasks scale down after load decreases

### Logging
- [ ] Container logs in CloudWatch
- [ ] Application logs visible
- [ ] No error messages
- [ ] Log retention configured
- [ ] Logs searchable

### Monitoring
- [ ] CloudWatch dashboard created
- [ ] Key metrics visible
- [ ] Alarms configured and armed
- [ ] Alarm test passes

---

## 📋 Documentation & Runbooks

### Documentation
- [ ] Deployment guide written
- [ ] Environment variables documented
- [ ] Architecture diagram created
- [ ] Network diagram created
- [ ] Runbooks created for:
  - [ ] Deployment
  - [ ] Scaling
  - [ ] Backup/restore
  - [ ] Troubleshooting
  - [ ] Rollback procedures

### Team Knowledge
- [ ] Team trained on deployment process
- [ ] Team trained on monitoring
- [ ] Team trained on troubleshooting
- [ ] On-call rotation established
- [ ] Escalation procedures defined

---

## 🔄 Maintenance & Updates

### Regular Tasks
- [ ] Daily: Monitor CloudWatch dashboard
- [ ] Weekly: Review logs for errors
- [ ] Monthly: Review security settings
- [ ] Quarterly: Update base images
- [ ] Annually: Review architecture

### Backup & Disaster Recovery
- [ ] RDS backups automated
- [ ] Backup retention adequate
- [ ] Restore procedure tested
- [ ] Cross-region backup (if needed)
- [ ] RPO/RTO documented

### Patches & Updates
- [ ] Base image updates monitored
- [ ] Security patches applied promptly
- [ ] Dependency updates tested
- [ ] Blue-green deployment process
- [ ] Rollback procedure tested

---

## ✅ Sign-Off

| Role | Name | Date | Status |
|------|------|------|--------|
| **Developer** | _______ | _______ | ☐ |
| **DevOps** | _______ | _______ | ☐ |
| **QA** | _______ | _______ | ☐ |
| **Manager** | _______ | _______ | ☐ |

---

## 📞 Support & Escalation

- **Deployment Issues**: Contact DevOps team
- **Application Errors**: Check CloudWatch logs
- **Database Issues**: Check RDS console
- **Scaling Issues**: Review Auto Scaling policies
- **Security Concerns**: Escalate to security team

---

**Deployment Date**: ________________
**Deployed By**: ________________
**Version**: v1.0.0
**Status**: ✅ Production Ready

---

**Last Updated:** March 17, 2026

