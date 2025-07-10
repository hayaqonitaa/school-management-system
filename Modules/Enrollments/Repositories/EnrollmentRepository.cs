using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Enrollments.Entities;

namespace SchoolManagementSystem.Modules.Enrollments.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly DatabaseConfig _context;

        public EnrollmentRepository(DatabaseConfig context)
        {
            _context = context;
        }

        public async Task<List<Enrollment>> GetAllAsync()
        {
            return await _context.Enrollments
                .FromSqlRaw(@"
                    SELECT e.*, s.*, ct.*, t.*, c.* 
                    FROM ""Enrollments"" e 
                    LEFT JOIN ""Students"" s ON e.""IdStudent"" = s.""Id""
                    LEFT JOIN ""ClassTeachers"" ct ON e.""IdClassTeacher"" = ct.""Id""
                    LEFT JOIN ""Teachers"" t ON ct.""IdTeacher"" = t.""Id""
                    LEFT JOIN ""Classes"" c ON ct.""IdClass"" = c.""Id""")
                .Include(e => e.Student)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Teacher)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Class)
                .ToListAsync();
        }

        public async Task<(List<Enrollment> enrollments, int totalCount)> GetAllPaginatedAsync(int page, int pageSize)
        {
            var offset = (page - 1) * pageSize;
            
            // Get total count
            var totalCount = await _context.Enrollments.CountAsync();

            // Get paginated data
            var enrollments = await _context.Enrollments
                .FromSqlRaw(@"
                    SELECT e.*, s.*, ct.*, t.*, c.* 
                    FROM ""Enrollments"" e 
                    LEFT JOIN ""Students"" s ON e.""IdStudent"" = s.""Id""
                    LEFT JOIN ""ClassTeachers"" ct ON e.""IdClassTeacher"" = ct.""Id""
                    LEFT JOIN ""Teachers"" t ON ct.""IdTeacher"" = t.""Id""
                    LEFT JOIN ""Classes"" c ON ct.""IdClass"" = c.""Id""
                    ORDER BY e.""CreatedAt"" DESC 
                    OFFSET {0} LIMIT {1}", offset, pageSize)
                .Include(e => e.Student)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Teacher)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Class)
                .ToListAsync();

            return (enrollments, totalCount);
        }

        public async Task<Enrollment?> GetByIdAsync(Guid id)
        {
            return await _context.Enrollments
                .FromSqlRaw(@"
                    SELECT e.*, s.*, ct.*, t.*, c.* 
                    FROM ""Enrollments"" e 
                    LEFT JOIN ""Students"" s ON e.""IdStudent"" = s.""Id""
                    LEFT JOIN ""ClassTeachers"" ct ON e.""IdClassTeacher"" = ct.""Id""
                    LEFT JOIN ""Teachers"" t ON ct.""IdTeacher"" = t.""Id""
                    LEFT JOIN ""Classes"" c ON ct.""IdClass"" = c.""Id""
                    WHERE e.""Id"" = {0}", id)
                .Include(e => e.Student)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Teacher)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Class)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Enrollment>> GetByStudentIdAsync(Guid studentId)
        {
            return await _context.Enrollments
                .FromSqlRaw(@"
                    SELECT e.*, s.*, ct.*, t.*, c.* 
                    FROM ""Enrollments"" e 
                    LEFT JOIN ""Students"" s ON e.""IdStudent"" = s.""Id""
                    LEFT JOIN ""ClassTeachers"" ct ON e.""IdClassTeacher"" = ct.""Id""
                    LEFT JOIN ""Teachers"" t ON ct.""IdTeacher"" = t.""Id""
                    LEFT JOIN ""Classes"" c ON ct.""IdClass"" = c.""Id""
                    WHERE e.""IdStudent"" = {0}", studentId)
                .Include(e => e.Student)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Teacher)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Class)
                .ToListAsync();
        }

        public async Task<List<Enrollment>> GetByClassTeacherIdAsync(Guid classTeacherId)
        {
            return await _context.Enrollments
                .FromSqlRaw(@"
                    SELECT e.*, s.*, ct.*, t.*, c.* 
                    FROM ""Enrollments"" e 
                    LEFT JOIN ""Students"" s ON e.""IdStudent"" = s.""Id""
                    LEFT JOIN ""ClassTeachers"" ct ON e.""IdClassTeacher"" = ct.""Id""
                    LEFT JOIN ""Teachers"" t ON ct.""IdTeacher"" = t.""Id""
                    LEFT JOIN ""Classes"" c ON ct.""IdClass"" = c.""Id""
                    WHERE e.""IdClassTeacher"" = {0}", classTeacherId)
                .Include(e => e.Student)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Teacher)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Class)
                .ToListAsync();
        }

        public async Task<Enrollment?> GetByStudentAndClassTeacherAsync(Guid studentId, Guid classTeacherId)
        {
            return await _context.Enrollments
                .FromSqlRaw(@"
                    SELECT e.*, s.*, ct.*, t.*, c.* 
                    FROM ""Enrollments"" e 
                    LEFT JOIN ""Students"" s ON e.""IdStudent"" = s.""Id""
                    LEFT JOIN ""ClassTeachers"" ct ON e.""IdClassTeacher"" = ct.""Id""
                    LEFT JOIN ""Teachers"" t ON ct.""IdTeacher"" = t.""Id""
                    LEFT JOIN ""Classes"" c ON ct.""IdClass"" = c.""Id""
                    WHERE e.""IdStudent"" = {0} AND e.""IdClassTeacher"" = {1}", studentId, classTeacherId)
                .Include(e => e.Student)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Teacher)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Class)
                .FirstOrDefaultAsync();
        }

        public async Task<Enrollment?> GetByStudentIdSingleAsync(Guid studentId)
        {
            return await _context.Enrollments
                .FromSqlRaw(@"
                    SELECT e.*, s.*, ct.*, t.*, c.* 
                    FROM ""Enrollments"" e 
                    LEFT JOIN ""Students"" s ON e.""IdStudent"" = s.""Id""
                    LEFT JOIN ""ClassTeachers"" ct ON e.""IdClassTeacher"" = ct.""Id""
                    LEFT JOIN ""Teachers"" t ON ct.""IdTeacher"" = t.""Id""
                    LEFT JOIN ""Classes"" c ON ct.""IdClass"" = c.""Id""
                    WHERE e.""IdStudent"" = {0}", studentId)
                .Include(e => e.Student)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Teacher)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Class)
                .FirstOrDefaultAsync();
        }

        public async Task<Enrollment> CreateAsync(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task<Enrollment> UpdateAsync(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var enrollment = await _context.Enrollments
                .FromSqlRaw("SELECT * FROM \"Enrollments\" WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();
            if (enrollment == null) return false;

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            var result = await _context.Database
                .SqlQueryRaw<int>("SELECT COUNT(*) FROM \"Enrollments\" WHERE \"Id\" = {0}", id)
                .FirstAsync();
            return result > 0;
        }

        public async Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid classTeacherId)
        {
            var result = await _context.Database
                .SqlQueryRaw<int>("SELECT COUNT(*) FROM \"Enrollments\" WHERE \"IdStudent\" = {0} AND \"IdClassTeacher\" = {1}", studentId, classTeacherId)
                .FirstAsync();
            return result > 0;
        }

        public async Task<bool> IsStudentEnrolledInAnyClassAsync(Guid studentId)
        {
            var result = await _context.Database
                .SqlQueryRaw<int>("SELECT COUNT(*) FROM \"Enrollments\" WHERE \"IdStudent\" = {0}", studentId)
                .FirstAsync();
            return result > 0;
        }
    }
}
