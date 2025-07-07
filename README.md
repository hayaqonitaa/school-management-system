# School Management System

A comprehensive school management system built with ASP.NET Core 8.0, featuring multi-role authentication, modular architecture, and role-based access control.

## Prerequisites

- .NET 8.0 SDK
- Visual Studio Code or Visual Studio 2022


## How to Run Locally

1. **Clone and navigate to project**
```bash
git clone <repository-url>
cd school-management-system
```

2. **Restore dependencies**
```bash
dotnet restore
```

3. **Run the application**
```bash
dotnet run
```

That's it! The application will be available at:
- HTTP: http://localhost:5037
- Swagger UI: http://localhost:5037/swagger

## Database Setup

### Quick Start (Recommended)

**Database is already configured in `appsettings.json`** - No setup needed!

### Alternative: Use Your Own Database
If you want to use your own Supabase database:

1. Create a Supabase project at https://supabase.com
2. Get your connection details from Settings > Database
3. Update the connection string in `appsettings.json` and `appsettings.Development.json`

### Run Migrations (If using your own database)
```bash
# Install EF Core tools (if not already installed)
dotnet tool install --global dotnet-ef

# Apply migrations (only needed if using your own database)
dotnet ef database update
```


## Default Admin Account

- Email: admin@school.com
- Password: admin123

## API Documentation

### Authentication

All API endpoints require authentication except for the login endpoint.

#### Login
```http
POST /auth/login
Content-Type: application/json

{
  "email": "admin@school.com",
  "password": "admin123"
}
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": 1,
  "email": "admin@school.com",
  "role": "Admin",
  "roleId": 1,
  "expiresAt": "2025-07-15T10:30:00Z"
}
```

#### Using JWT Token
Include the token in the Authorization header:
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Role-Based Access Control

#### Admin (Full Access)
- Can perform all CRUD operations on all entities
- Can manage users, students, teachers, classes, and enrollments

#### Teacher (Limited Access)
- Can view classes and enrollments for their assigned classes only
- Can create and update enrollments for their assigned classes
- Cannot delete enrollments or manage other resources

#### Student (Read-Only Access)
- Can view their own enrollment information
- Can view their class information
- Cannot modify any data

### API Endpoints

#### Students
```http
# Get all students (Admin only)
GET /api/students
Authorization: Bearer {admin_token}

# Get student by ID (Admin only)
GET /api/students/{id}
Authorization: Bearer {admin_token}

# Create student (Admin only)
POST /api/students
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "fullName": "John Doe",
  "email": "john.doe@student.com",
  "dateOfBirth": "2005-05-15",
  "phoneNumber": "1234567890",
  "address": "123 Main St",
  "guardianName": "Jane Doe",
  "guardianPhoneNumber": "0987654321"
}
```

#### Teachers
```http
# Get all teachers (Admin only)
GET /api/teachers
Authorization: Bearer {admin_token}

# Create teacher (Admin only)
POST /api/teachers
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "nip": "200402112023011001",
  "fullName": "Dr. Smith",
  "alamat": "456 Teacher St",
  "email": "dr.smith@school.com",
  "password": "teacher123",
  "phoneNumber": "5555555555"
}
```

#### Classes
```http
# Get all classes (Admin and Teacher)
GET /api/classes
Authorization: Bearer {token}

# Create class (Admin only)
POST /api/classes
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "kelas": "10A",
  "tingkat": 10,
  "capacity": 30
}
```

#### Class Teacher Assignments
```http
# Assign teacher to class (Admin only)
POST /api/classteachers
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "idTeacher": "teacher-guid-here",
  "idClass": "class-guid-here",
  "tahun": 2025
}

# Get teacher's assignments
GET /api/classteachers/teacher/{teacherId}
Authorization: Bearer {token}
```

#### Enrollments
```http
# Get all enrollments (Admin only)
GET /api/enrollments
Authorization: Bearer {admin_token}

# Get teacher's class enrollments (Teacher only)
GET /api/enrollments/my-classes
Authorization: Bearer {teacher_token}

# Get student's enrollment (Student only)
GET /api/enrollments/my-enrollment
Authorization: Bearer {student_token}

# Create enrollment (Admin and Teacher)
POST /api/enrollments
Authorization: Bearer {token}
Content-Type: application/json

{
  "idStudent": "student-guid-here",
  "idClassTeacher": "classteacher-guid-here"
}

# Update enrollment (Admin and Teacher)
PUT /api/enrollments/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "idStudent": "student-guid-here",
  "idClassTeacher": "classteacher-guid-here"
}

# Delete enrollment (Admin only)
DELETE /api/enrollments/{id}
Authorization: Bearer {admin_token}
```

## Environment Configuration

The application supports environment configuration through `.env` file:

### Current Setup (Ready to Use)
The project includes a pre-configured `.env` file with shared database credentials. No additional setup required for testing.

### Environment Variables
The application reads these environment variables:

```properties
# Environment
ASPNETCORE_ENVIRONMENT=Development

# Database Configuration
ConnectionStrings__DefaultConnection=Server=db.wykmilshndgfmrsnbhat.supabase.co;Port=5432;Database=postgres;User Id=postgres;Password=Kucinghitam12;SSL Mode=Require;

# JWT Configuration
Jwt__Key=SchoolManagementSystemSecretKeyForJWTTokenGeneration123456789
Jwt__Issuer=SchoolManagementSystem
Jwt__Audience=SchoolManagementSystem

# Application Settings
ASPNETCORE_URLS=http://localhost:5037
```