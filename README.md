# FTM Backend (FTM-BE)

## 📋 Giới thiệu / Overview

**FTM (Finance & Transaction Management)** là một hệ thống backend được xây dựng bằng ASP.NET Core 7.0, áp dụng Clean Architecture pattern để quản lý tài chính và giao dịch. Dự án thuộc về Capstone Project - Fall 2025 của FPT University.

**FTM (Finance & Transaction Management)** is a backend system built with ASP.NET Core 7.0, applying Clean Architecture pattern for financial and transaction management. This project is part of FPT University's Capstone Project - Fall 2025.

## 🏗️ Kiến trúc / Architecture

Dự án được tổ chức theo Clean Architecture với 4 layers chính:

```
FTM-BE/
├── FTM.API/              # Presentation Layer - Web API
├── FTM.Application/      # Application Layer - Business Logic
├── FTM.Domain/           # Domain Layer - Entities & Interfaces
└── FTM.Infrastructure/   # Infrastructure Layer - Data Access
```

### 1. **FTM.API** - Presentation Layer
- **Controllers**: RESTful API endpoints
- **DTOs**: Data Transfer Objects
- **Middleware**: Custom middleware (Logger, etc.)
- **Extensions**: Service configuration extensions
- **Helpers**: Utility classes for data validation and formatting
  - `EmailSensitive`: Email validation
  - `PhoneSensitive`: Phone number formatting (Vietnamese format)
  - `UsernameSensitive`: Username validation
  - `StringHelper`: String manipulation utilities

### 2. **FTM.Application** - Application Layer
- **Services**: Business logic implementation
- **IServices**: Service interfaces
- **Helpers**: Application-level utilities

### 3. **FTM.Domain** - Domain Layer
- **Entities**: Domain models
  - `BaseEntity`: Base class with common properties (Id, CreatedOn, LastModifiedOn, IsDeleted, etc.)
- **Interfaces**: Repository and service contracts
- **Specifications**: Query specification pattern implementation
  - `ISpecification<T>`: Generic specification interface
  - `BaseSpecification<T>`: Base implementation
  - `BaseSpecParams`: Pagination and filtering parameters
  - `PropertyFilter`: Dynamic property filtering

### 4. **FTM.Infrastructure** - Infrastructure Layer
- **Data**: DbContext configurations
  - `FTMDbContext`: Main application database context
  - `AppIdentityDbContext`: Identity and authentication database context
- **Repositories**: Data access implementations
- **Migrations**: Database migration files

## 🛠️ Công nghệ / Technologies

### Core Framework
- **.NET 7.0**: Primary framework (Note: Net7.0 is EOL - consider upgrading to .NET 8.0+)
- **ASP.NET Core**: Web framework
- **Entity Framework Core 7.0**: ORM

### Database
- **PostgreSQL**: Primary database with Npgsql provider
- **Dual Database Architecture**:
  - `test_gp_identity`: Authentication and authorization
  - `test_gp`: Application data

### Libraries & Packages
- **Swagger/OpenAPI**: API documentation
- **Newtonsoft.Json**: JSON serialization
- **Microsoft.AspNetCore.Identity**: Authentication/Authorization
- **Health Checks**: Database health monitoring
- **Microsoft.Data.SqlClient**: SQL Server support

### Design Patterns
- **Repository Pattern**: Data access abstraction
- **Specification Pattern**: Flexible query building
- **Dependency Injection**: Built-in IoC container
- **Middleware Pattern**: Request/response pipeline
- **Clean Architecture**: Separation of concerns

## 🚀 Cài đặt / Installation

### Prerequisites
```bash
# .NET SDK 7.0 or higher
dotnet --version

# PostgreSQL 12+
psql --version
```

### Setup Steps

1. **Clone repository**
```bash
git clone https://github.com/FPT-Capstone-Project-Fall-2025/FTM-BE.git
cd FTM-BE
```

2. **Restore packages**
```bash
dotnet restore
```

3. **Update database connection** (if needed)

Modify connection string in `FTM.API/Extensions/FTMAppContextExtensions.cs`:
```csharp
// Current configuration
Host = "128.199.168.119"
Port = 5432
Database = "test_gp" / "test_gp_identity"
Username = "appuser"
Password = "secret@123"
```

4. **Build solution**
```bash
dotnet build
```

5. **Run migrations** (if any)
```bash
dotnet ef database update --project FTM.Infrastructure --startup-project FTM.API
```

6. **Run application**
```bash
cd FTM.API
dotnet run
```

Application will start at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

## 📡 API Endpoints

### Health Check
- `GET /health` - Check database connection status

### Dummy Endpoint (Example)
- `GET /` - Returns a test message

### Future Endpoints
(To be implemented based on business requirements)

## 🔧 Configuration

### appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Database Configuration
- **Connection Pooling**: Enabled
- **Command Timeout**: 300 seconds
- **Retry on Failure**: 5 attempts with 10-second intervals
- **Service Lifetime**: Transient (for both DbContexts)

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## 📝 Development Guidelines

### Code Structure
- Follow Clean Architecture principles
- Use Repository pattern for data access
- Implement Specification pattern for complex queries
- Apply SOLID principles
- Use Dependency Injection

### Entity Guidelines
- All entities should inherit from `BaseEntity`
- Implement soft delete using `IsDeleted` flag
- Track creation and modification metadata

### API Guidelines
- Use async/await for all I/O operations
- Implement proper error handling
- Use DTOs for data transfer
- Document APIs with XML comments for Swagger

### Middleware
- **LoggerMiddleware**: Logs all HTTP requests/responses with timing information

## 🔐 Security

- ASP.NET Core Identity for authentication
- Separate database for identity management
- Connection string encryption (recommended for production)
- SSL/TLS support with trusted certificates

## 📊 Database Schema

### BaseEntity (Common Fields)
```csharp
- Id: Guid (Primary Key)
- CreatedOn: DateTimeOffset
- CreatedBy: string
- CreatedByUserId: Guid
- LastModifiedOn: DateTimeOffset
- LastModifiedBy: string
- IsDeleted: bool?
```

## 🤝 Contributing

This is a Capstone Project for FPT University. For contribution guidelines, please contact the project team.

## 📄 License

This project is part of FPT University's academic program.

## 👥 Team

**FPT Capstone Project - Fall 2025**

## 📞 Support

For issues and questions, please create an issue in the GitHub repository.

## 🔄 Project Status

**Current Status**: In Development
- ✅ Project structure established
- ✅ Clean Architecture implemented
- ✅ Database contexts configured
- ✅ Health checks implemented
- ✅ Logging middleware active
- 🔄 Business features in progress

## 📈 Roadmap

- [ ] Implement authentication/authorization flows
- [ ] Add business domain entities
- [ ] Implement core API endpoints
- [ ] Add comprehensive unit tests
- [ ] Integrate CI/CD pipeline
- [ ] Deploy to staging environment
- [ ] Performance optimization
- [ ] Security audit

## ⚠️ Important Notes

1. **Target Framework**: Currently using .NET 7.0 which is End of Life (EOL). Consider upgrading to .NET 8.0 or higher for security updates.
2. **Database Credentials**: Change default credentials before deploying to production.
3. **CORS**: Configure CORS policies based on frontend requirements.
4. **Environment Variables**: Use environment variables for sensitive configuration in production.

---

**Last Updated**: November 2025  
**Version**: 0.1.0 (Development)