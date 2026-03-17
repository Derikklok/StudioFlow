# 🔐 Environment Variables Reference

Complete guide to all environment variables used in StudioFlow deployment.

---

## 📋 Table of Contents

1. [Database Configuration](#database-configuration)
2. [API Configuration](#api-configuration)
3. [ASP.NET Core Settings](#aspnet-core-settings)
4. [Logging Configuration](#logging-configuration)
5. [AWS-Specific Variables](#aws-specific-variables)
6. [Security Considerations](#security-considerations)

---

## Database Configuration

### `DB_HOST`
- **Purpose**: Database server hostname/IP
- **Type**: String
- **Default**: `mysql` (Docker Compose) | RDS endpoint (AWS)
- **Examples**:
  ```
  Local:      mysql
  AWS RDS:    studioflow-db.xxxxxx.us-east-1.rds.amazonaws.com
  IP:         192.168.1.100
  ```
- **Connection String**: `server={DB_HOST};...`

### `DB_PORT`
- **Purpose**: Database server port
- **Type**: Integer
- **Default**: `3306`
- **Valid Range**: 1-65535
- **Change If**: Using non-standard MySQL port
- **Example**: `3306`

### `DB_NAME`
- **Purpose**: Database name
- **Type**: String
- **Default**: `studio_db`
- **Constraints**: Alphanumeric, underscores only
- **Example**: `studio_db`, `studioflow_prod`

### `DB_USER`
- **Purpose**: Database username
- **Type**: String
- **Default**: `root` (local) | `admin` (AWS)
- **Note**: Should NOT be root in production
- **Example**: `studioflow`, `app_user`

### `DB_PASSWORD`
- **Purpose**: Database password
- **Type**: String
- **Default**: `sachin1605` (development only)
- **Requirements**:
  - Minimum 12 characters in production
  - Mix of upper, lower, numbers, special characters
  - No quotes or special shell characters
- **⚠️ SECURITY**: Never commit to git, use Secrets Manager

### Connection String Format

```
server={DB_HOST};database={DB_NAME};user={DB_USER};password={DB_PASSWORD}
```

**Example**:
```
server=mysql;database=studio_db;user=root;password=mysecurepass123
```

---

## API Configuration

### `API_PORT`
- **Purpose**: Port where API listens (HTTP)
- **Type**: Integer
- **Default**: `5000`
- **Docker Mapping**: `5000:80` (local port : container port)
- **Valid Range**: 1024-65535 (>1024 requires elevated permissions)
- **Examples**:
  ```
  Development:    5000
  Testing:        8000
  Production:     80 (via load balancer)
  ```

### `API_HTTPS_PORT`
- **Purpose**: Port where API listens (HTTPS)
- **Type**: Integer
- **Default**: `5001`
- **Docker Mapping**: `5001:443`
- **Note**: Typically not used directly, HTTPS via ALB/proxy
- **Example**: `5001`

### `ASPNETCORE_URLS`
- **Purpose**: URLs the API binds to
- **Type**: String
- **Default**: `http://+:80`
- **Format**: `protocol://host:port`
- **Examples**:
  ```
  http://+:80               (HTTP on all interfaces)
  https://+:443             (HTTPS on all interfaces)
  http://localhost:5000     (Specific interface)
  http://+:80;https://+:443 (Multiple URLs)
  ```

---

## ASP.NET Core Settings

### `ASPNETCORE_ENVIRONMENT`
- **Purpose**: Runtime environment
- **Type**: String
- **Allowed Values**:
  - `Development` - Full logging, detailed errors
  - `Staging` - Production-like, debug info available
  - `Production` - Minimal logging, optimized
- **Default**: `Development`
- **Impact**:
  - Error pages (Dev vs generic)
  - Logging verbosity
  - Performance optimizations
  - Swagger/API docs visibility

**Development Example**:
```
ASPNETCORE_ENVIRONMENT=Development
Logging__LogLevel__Default=Debug
```

**Production Example**:
```
ASPNETCORE_ENVIRONMENT=Production
Logging__LogLevel__Default=Warning
```

---

## Logging Configuration

### `Logging__LogLevel__Default`
- **Purpose**: Default logging level
- **Type**: String
- **Allowed Values** (most to least verbose):
  - `Trace` - Most detailed, rarely needed
  - `Debug` - Development debugging
  - `Information` - General information (default)
  - `Warning` - Warning messages only
  - `Error` - Errors only
  - `Critical` - Critical errors only
  - `None` - No logging
- **Default**: `Information`
- **Production**: Recommend `Warning` or `Error`

**Examples**:
```yaml
Development:  Information  # Normal logging
Testing:      Debug        # Detailed info
Production:   Warning      # Minimal noise
```

### `Logging__LogLevel__Microsoft__AspNetCore`
- **Purpose**: ASP.NET Core framework logging
- **Type**: String
- **Allowed Values**: Same as above
- **Default**: `Warning`
- **Recommendation**: Keep at `Warning` in production
- **Example**: `Warning`

### Log Output Locations

```
Docker Container:
  └─ stdout/stderr (captured by Docker)
     └─ CloudWatch (via awslogs driver)
     
File System:
  └─ /app/logs/ (in container)
     └─ ./logs/ (mounted locally)
     
AWS:
  └─ CloudWatch Logs group: /ecs/studioflow
```

---

## AWS-Specific Variables

### `AWS_REGION`
- **Purpose**: AWS region for services
- **Type**: String
- **Default**: `us-east-1`
- **Examples**: `us-east-1`, `us-west-2`, `eu-west-1`
- **Usage**: AWS CLI commands, resource ARNs

### `AWS_ACCOUNT_ID`
- **Purpose**: AWS account ID
- **Type**: String (12 digits)
- **Example**: `123456789012`
- **Usage**: ECR registry URL, IAM ARNs

### `ECR_REGISTRY`
- **Purpose**: ECR registry URL
- **Type**: String
- **Format**: `{ACCOUNT_ID}.dkr.ecr.{REGION}.amazonaws.com`
- **Example**: `123456789012.dkr.ecr.us-east-1.amazonaws.com`
- **Usage**: Docker image tagging and pushing

---

## Full Configuration Examples

### Local Development

```bash
# .env (development)
DB_HOST=mysql
DB_PORT=3306
DB_NAME=studio_db
DB_USER=root
DB_PASSWORD=sachin1605

API_PORT=5000
API_HTTPS_PORT=5001

ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:80

Logging__LogLevel__Default=Information
Logging__LogLevel__Microsoft__AspNetCore=Warning
```

### Docker Testing

```bash
# .env (testing)
DB_HOST=mysql
DB_PORT=3306
DB_NAME=studio_db_test
DB_USER=testuser
DB_PASSWORD=testpass1234567890

API_PORT=8000
API_HTTPS_PORT=8001

ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:80

Logging__LogLevel__Default=Debug
Logging__LogLevel__Microsoft__AspNetCore=Warning
```

### AWS Production

```bash
# .env.aws (production)
DB_HOST=studioflow-db.xxxxx.us-east-1.rds.amazonaws.com
DB_PORT=3306
DB_NAME=studio_db
DB_USER=studioflow
DB_PASSWORD=${DB_PASSWORD_SECRET}  # From Secrets Manager

API_PORT=80
API_HTTPS_PORT=443

ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:80

Logging__LogLevel__Default=Warning
Logging__LogLevel__Microsoft__AspNetCore=Warning

AWS_REGION=us-east-1
AWS_ACCOUNT_ID=123456789012
ECR_REGISTRY=123456789012.dkr.ecr.us-east-1.amazonaws.com
```

---

## 🔐 Security Best Practices

### Secrets Management

**❌ DON'T**:
```bash
# Hardcoded in .env (committed to git)
DB_PASSWORD=mysecretpassword123

# In environment variable in code
password = Environment.GetEnvironmentVariable("PASSWORD");

# In Docker image
RUN export PASSWORD=mysecret
```

**✅ DO**:
```bash
# Use AWS Secrets Manager
aws secretsmanager create-secret \
  --name studioflow/db-password \
  --secret-string "your-secure-password"

# Reference in task definition
"secrets": [{
  "name": "DB_PASSWORD",
  "valueFrom": "arn:aws:secretsmanager:..."
}]

# Access in container
string password = Environment.GetEnvironmentVariable("DB_PASSWORD");
```

### Password Requirements

**Development** (testing only):
- Any string works
- Use `sachin1605` (test password)

**Production** (must have):
- Minimum 12 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one number
- At least one special character (!@#$%^&*)
- No repeated characters (max 2x)
- Not the same as username or email

**Example**:
```
Good:   P@ssw0rd!Secure123
Bad:    password123
Bad:    Password
Bad:    P@ssw0rd@ssw0rd (repeated)
```

### Environment Variable Security

1. **Never commit .env to git**
   ```bash
   # .gitignore
   .env
   .env.local
   .env.*.local
   ```

2. **Use separate files per environment**
   ```
   .env              (development - default)
   .env.aws         (AWS production)
   .env.staging     (staging environment)
   .env.example     (template only)
   ```

3. **Rotate secrets regularly**
   - Update Secrets Manager monthly
   - Notify team of changes
   - Document rotation schedule

4. **Limit access**
   - Only admins can access production .env
   - Use IAM policies for AWS access
   - Audit secret access logs

---

## 🔧 Changing Variables

### Local (Docker Compose)

```bash
# Edit .env file
nano .env

# Update variable
API_PORT=8000

# Restart service
docker-compose restart studioflow

# Verify
curl http://localhost:8000/health
```

### AWS (ECS)

```bash
# Update task definition
aws ecs register-task-definition \
  --cli-input-json file://updated-task-def.json

# Update service to use new task definition
aws ecs update-service \
  --cluster studioflow \
  --service studioflow-service \
  --force-new-deployment
```

### AWS (Secrets Manager)

```bash
# Update secret
aws secretsmanager put-secret-value \
  --secret-id studioflow/db-password \
  --secret-string "new-password-here"

# ECS will automatically use updated secret
# No redeployment needed
```

---

## 📊 Variable Validation

### Connection String Test

```bash
# From within container
docker exec studioflow-api bash -c \
  'mysql -h $DB_HOST -u $DB_USER -p$DB_PASSWORD -e "SELECT 1"'

# Should output: 1
```

### Environment Variable Check

```bash
# List all environment variables in container
docker exec studioflow-api env | sort

# Check specific variable
docker exec studioflow-api printenv DB_HOST
```

---

## 🆘 Common Issues

| Issue | Cause | Solution |
|-------|-------|----------|
| **Connection refused** | Wrong DB_HOST | Verify hostname in .env |
| **Access denied** | Wrong password | Check DB_PASSWORD |
| **Port in use** | Port already bound | Change API_PORT |
| **Silent failures** | Production mode hiding errors | Set LOG_LEVEL to Information |
| **Logs empty** | Wrong log level | Decrease verbosity |

---

## ✅ Verification Checklist

- [ ] DB_HOST points to correct database
- [ ] DB_PORT is accessible
- [ ] DB_USER exists with correct permissions
- [ ] DB_PASSWORD is secure (production)
- [ ] API_PORT doesn't conflict
- [ ] ASPNETCORE_ENVIRONMENT matches deployment
- [ ] LOG_LEVEL appropriate for environment
- [ ] .env is in .gitignore
- [ ] AWS variables set (if using AWS)
- [ ] Secrets Manager configured (if production)

---

**Status:** ✅ Complete Reference
**Updated:** March 17, 2026

