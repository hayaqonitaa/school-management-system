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
                .Include(ct => ct.Teacher)
                .Include(ct => ct.Class)
                .ToListAsync();
        }

        public async Task<ClassTeacher?> GetByIdAsync(Guid id)
        {
            return await _context.ClassTeachers
                .Include(ct => ct.Teacher)
                .Include(ct => ct.Class)
                .FirstOrDefaultAsync(ct => ct.Id == id);
        }

        public async Task<List<ClassTeacher>> GetByTeacherIdAsync(Guid teacherId)
        {
            return await _context.ClassTeachers
                .Include(ct => ct.Teacher)
                .Include(ct => ct.Class)
                .Where(ct => ct.IdTeacher == teacherId)
                .ToListAsync();
        }

        public async Task<List<ClassTeacher>> GetByClassIdAsync(Guid classId)
        {
            return await _context.ClassTeachers
                .Include(ct => ct.Teacher)
                .Include(ct => ct.Class)
                .Where(ct => ct.IdClass == classId)
                .ToListAsync();
        }

        public async Task<List<ClassTeacher>> GetByTahunAsync(int tahun)
        {
            return await _context.ClassTeachers
                .Include(ct => ct.Teacher)
                .Include(ct => ct.Class)
                .Where(ct => ct.Tahun == tahun)
                .ToListAsync();
        }

        public async Task<List<ClassTeacher>> GetByStatusAsync(AssignmentStatus status)
        {
            return await _context.ClassTeachers
                .Include(ct => ct.Teacher)
                .Include(ct => ct.Class)
                .Where(ct => ct.Status == status)
                .ToListAsync();
        }

        public async Task<ClassTeacher?> GetByTeacherAndYearAsync(Guid teacherId, int tahun)
        {
            return await _context.ClassTeachers
                .Include(ct => ct.Teacher)
                .Include(ct => ct.Class)
                .FirstOrDefaultAsync(ct => ct.IdTeacher == teacherId && ct.Tahun == tahun);
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
            var classTeacher = await GetByIdAsync(id);
            if (classTeacher == null) return false;

            _context.ClassTeachers.Remove(classTeacher);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.ClassTeachers.AnyAsync(ct => ct.Id == id);
        }
    }
}
