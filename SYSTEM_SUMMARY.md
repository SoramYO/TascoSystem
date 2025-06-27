# TascoSystem - Tóm Tắt Hệ Thống

## 🚀 Quick Start Commands

docker-compose up -d --build

## 📋 Services & Ports

| Service | Port | Database | Container |
|---------|------|----------|-----------|
| Gateway | 5000 | - | tascosystem-gateway |
| UserAuthService | 5001 | SQL Server (1434) | tascosystem-userauthservice |
| ProjectService | 5002 | PostgreSQL (5432) | tascosystem-projectservice |
| TaskService | 5003 | SQL Server (1435) | tascosystem-taskservice |
| Orchestrator | 5004 | - | tascosystem-orchestrator |
| PushMessageQueue | 5005 | - | tascosystem-pushmessagequeue |
| NotificationService | 5006 | - | tascosystem-notificationservice |

## 🗄️ Database Configuration

### SQL Server Databases
- **UserAuthService**: `TascoAuthDb` (Port 1434)
  - Connection: `Server=sqlserver-auth;Database=TascoAuthDb;User Id=sa;Password=Password123@;TrustServerCertificate=True`
  
- **TaskService**: `TascoTaskDb` (Port 1435)  
  - Connection: `Server=sqlserver-task;Database=TascoTaskDb;User Id=sa;Password=Password123@;TrustServerCertificate=True`

### PostgreSQL Database
- **ProjectService**: `ProjectManagementDB` (Port 5432)
  - Connection: `Host=postgresql;Database=ProjectManagementDB;Username=postgres;Password=12345`

## 🔧 Database Initialization

### SQL Scripts
- `init-auth-database.sql` - Tạo database cho UserAuthService
- `init-task-database.sql` - Tạo database cho TaskService  

### Chạy Scripts (Sau khi container database khởi động)
```bash
# Khởi tạo UserAuthService database
docker exec -i sqlserver-auth /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Password123@" -i init-auth-database.sql

# Khởi tạo TaskService database  
docker exec -i sqlserver-task /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Password123@" -i init-task-database.sql
```

## 🏗️ Build Scripts

### build-step-by-step.bat
- Build từng service theo thứ tự
- Context paths cho từng service
- Hiển thị kết quả build (Success/Fail)

### check-images.bat  
- Kiểm tra tất cả Docker images
- Hiển thị status (Found/Missing)
- Summary tổng quan

## 🎯 Key Features

### Database Isolation
- Mỗi service có database riêng
- Không ảnh hưởng lẫn nhau
- Có thể scale độc lập

### Multi-Stage Docker Builds
- Base image: `mcr.microsoft.com/dotnet/aspnet:8.0`
- Build image: `mcr.microsoft.com/dotnet/sdk:8.0`
- Optimized production images

### Infrastructure Services
- **RabbitMQ**: Message broker (Port 5672, UI: 15672)
- **Redis**: Caching (Port 6379)

## 🔍 Troubleshooting

### Common Issues
1. **Docker Desktop không chạy** → Start Docker Desktop
2. **Port conflict** → Kiểm tra port đang sử dụng
3. **Build fail** → Kiểm tra Dockerfile và project references
4. **HTTPS certificate error** → Đã tắt HTTPS, chỉ dùng HTTP

### Useful Commands
```bash
# Xem containers đang chạy
docker ps

# Xem logs service
docker logs [container-name]

# Xem tất cả images
docker images | findstr tascosystem

# Clean up
docker image prune -f

```

## 📊 Current Status
✅ **All Docker images built successfully**
- Tất cả 7 services đã build thành công
- Database scripts sẵn sàng
- System ready to run

## 📁 Clean Project Structure
```
TascoSystem/
├── SYSTEM_SUMMARY.md          # 📖 Tóm tắt hệ thống (này)
├── docker-compose.yml         # 🐳 Docker Compose configuration
├── build-step-by-step.bat     # 🔨 Build tất cả services
├── check-images.bat           # ✅ Kiểm tra Docker images
├── init-auth-database.sql     # 🗄️ Khởi tạo UserAuthService DB
├── init-task-database.sql     # 🗄️ Khởi tạo TaskService DB
├── .dockerignore              # 🚫 Docker ignore rules
├── .gitignore                 # 🚫 Git ignore rules
└── services/                  # 📁 Source code các services
    ├── Tasco.Gateway/
    ├── Tasco.UserAuthService/
    ├── Tasco.ProjectService/
    ├── Tasco.TaskService/
    ├── Tasco.Orchestrator/
    ├── PushMessageQueue/
    └── Tasco.NotificationService/
``` 