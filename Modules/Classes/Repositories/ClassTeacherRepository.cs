using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Classes.Entities;

namespace SchoolManagementSystem.Modules.Classes.Repositories
{
    public class ClassTeacherRepository : IClassTeacherRepository
    {
        private readonly DatabaseConfig _context;

        public ClassTeacherRepository(DatabaseConfig context)
        {
            _context = context;
        }

        public async Task<List<ClassTeacher>> GetAllAsync()
        {
            return await _context.ClassTeachers
                .FromSqlRaw(@"
                    SELECT ct.*, t.*, c.* 
                    FROM ""ClassTeachers"" ct 
                    LEFT JOIN ""Teachers"" t ON ct.""IdTeacher"" = t.""Id"" 
                    LEFT JOIN ""Classes"" c ON ct.""IdClass"" = c.""Id""")
                .Include(ct => ct.Teacher)
                .Include(ct => ct.Class)
                .ToListAsync();
        }

        public async Task<ClassTeacher?> GetByIdAsync(Guid id)
        {
            return await _context.ClassTeachers
                .FromSqlRaw(@"
                    SELECT ct.*, t.*, c.* 
                    FROM ""ClassTeachers"" ct 
                    LEFT JOIN ""Teachers"" t ON ct.""IdTeacher"" = t.""Id"" 
                    LEFT JOIN ""Classes"" c ON ct.""IdClass"" = c.""Id"" 
                    WHERE ct.""Id"" = {0}", id)
                .Include(ct => ct.Teacher)
                .Include(ct => ct.Class)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ClassTeacher>> GetByTeacherIdAsync(Guid teacherId)
        {
            return await _context.ClassTeachers
                .FromSqlRaw(@"
                    SELECT ct.*, t.*, c.* 
                    FROM ""ClassTeachers"" ct 
                    LEFT JOIN ""Teachers"" t ON ct.""IdTeacher"" = t.""Id"" 
                    LEFT JOIN ""Classes"" c ON ct.""IdClass"" = c.""Id"" 
                    WHERE ct.""IdTeacher"" = {0}", teacherId)
                .Include(ct => ct.Teacher)
                .Include(ct => ct.Class)
                .ToListAsync();
        }

        public async Task<List<ClassTeacher>> GetByClassIdAsync(Guid classId)
        {
            return await _context.ClassTeachers
                .FromSqlRaw(@"
                    SELECT ct.*, t.*, c.* 
                    FROM ""ClassTeachers"" ct 
                    LEFT JOIN ""Teachers"" t ON ct.""IdTeacher"" = t.""Id"" 
                    LEFT JOIN ""Classes"" c ON ct.""IdClass"" = c.""Id"" 
                    WHERE ct.""IdClass"" = {0}", classId)
                .Include(ct => ct.Teacher)
                .Include(ct => ct.Class)
                .ToListAsync();
        }

        public async Task<List<ClassTeacher>> GetByTahunAsync(int tahun)
        {
            return await _context.ClassTeachers
                .FromSqlRaw(@"
                    SELECT ct.*, t.*, c.* 
                    FROM ""ClassTeachers"" ct 
                    LEFT JOIN ""Teachers"" t ON ct.""IdTeacher"" = t.""Id"" 
                    LEFT JOIN ""Classes"" c ON ct.""IdClass"" = c.""Id"" 
                    WHERE ct.""Tahun"" = {0}", tahun)
                .Include(ct => ct.Teacher)
                .Include(ct => ct.Class)
                .ToListAsync();
        }

        public async Task<List<ClassTeacher>> GetByStatusAsync(AssignmentStatus status)
        {
            return await _context.ClassTeachers
                .FromSqlRaw(@"
                    SELECT ct.*, t.*, c.* 
                    FROM ""ClassTeachers"" ct 
                    LEFT JOIN ""Teachers"" t ON ct.""IdTeacher"" = t.""Id"" 
                    LEFT JOIN ""Classes"" c ON ct.""IdClass"" = c.""Id"" 
                    WHERE ct.""Status"" = {0}", (int)status)
                .Include(ct => ct.Teacher)
                .Include(ct => ct.Class)
                .ToListAsync();
        }

        public async Task<ClassTeacher?> GetByTeacherAndYearAsync(Guid teacherId, int tahun)
        {
            return await _context.ClassTeachers
                .FromSqlRaw(@"
                    SELECT ct.*, t.*, c.* 
                    FROM ""ClassTeachers"" ct 
                    LEFT JOIN ""Teachers"" t ON ct.""IdTeacher"" = t.""Id"" 
                    LEFT JOIN ""Classes"" c ON ct.""IdClass"" = c.""Id"" 
                    WHERE ct.""IdTeacher"" = {0} AND ct.""Tahun"" = {1}", teacherId, tahun)
                .Include(ct => ct.Teacher)
                .Include(ct => ct.Class)
                .FirstOrDefaultAsync();
        }

        public async Task<ClassTeacher> CreateAsync(ClassTeacher classTeacher)
        {
            _context.ClassTeachers.Add(classTeacher);
            await _context.SaveChangesAsync();
            return classTeacher;
        }

        public async Task<ClassTeacher> UpdateAsync(ClassTeacher classTeacher)
        {
            _context.ClassTeachers.Update(classTeacher);
            await _context.SaveChangesAsync();
            return classTeacher;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var classTeacher = await _context.ClassTeachers
                .FromSqlRaw("SELECT * FROM \"ClassTeachers\" WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();
            if (classTeacher == null) return false;

            _context.ClassTeachers.Remove(classTeacher);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            var result = await _context.Database
                .SqlQueryRaw<int>("SELECT COUNT(*) FROM \"ClassTeachers\" WHERE \"Id\" = {0}", id)
                .FirstAsync();
            return result > 0;
        }
    }
}
