# School Management System

A comprehensive school management system built with ASP.NET Core 8.0, featuring multi-role authentication, modular architecture, and role-based access control.

## Prerequisites

- .NET 8.0 SDK
- Visual Studio Code or Visual Studio 2022

## Required NuGet Packages

This project includes all necessary packages. If you're creating a similar project from scratch, you'll need:

```bash
# Entity Framework Core for PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

# JWT Authentication
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

# Password Hashing
dotnet add package BCrypt.Net-Next

# Environment Variables Support
dotnet add package DotNetEnv

# API Documentation (Swagger)
dotnet add package Microsoft.AspNetCore.OpenApi
dotnet add package Swashbuckle.AspNetCore
```

**Note:** All packages are already installed in this project.


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

**Database is already configured in `.env` file** - No setup needed!

### Alternative: Use Your Own Database
If you want to use your own Supabase database:

1. Create a Supabase project at https://supabase.com
2. Get your connection details from Settings > Database
3. Update the environment variables in `.env` file:
   ```env
   DB_HOST=your-supabase-host
   DB_PORT=5432
   DB_NAME=postgres
   DB_USERNAME=your-username
   DB_PASSWORD=your-password
   ```

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

The application uses environment variables for configuration through a `.env` file for better security and flexibility.

### Setup Environment Variables

1. **Copy the example file:**
```bash
cp .env.example .env
```

2. **Update the `.env` file with your configuration:**
```env
# Database Configuration
DB_HOST=aws-0-ap-southeast-1.pooler.supabase.com
DB_PORT=5432
DB_NAME=postgres
DB_USERNAME=postgres.wykmilshndgfmrsnbhat
DB_PASSWORD=Kucinghitam12

# JWT Configuration
JWT_KEY=SchoolManagementSystemSecretKeyForJWTTokenGeneration123456789
JWT_ISSUER=SchoolManagementSystem
JWT_AUDIENCE=SchoolManagementSystem
```