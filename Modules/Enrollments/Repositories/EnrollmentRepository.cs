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
                .Include(e => e.Student)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Teacher)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Class)
                .ToListAsync();
        }

        public async Task<Enrollment?> GetByIdAsync(Guid id)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Teacher)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Class)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Enrollment>> GetByStudentIdAsync(Guid studentId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Teacher)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Class)
                .Where(e => e.IdStudent == studentId)
                .ToListAsync();
        }

        public async Task<List<Enrollment>> GetByClassTeacherIdAsync(Guid classTeacherId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Teacher)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Class)
                .Where(e => e.IdClassTeacher == classTeacherId)
                .ToListAsync();
        }

        public async Task<Enrollment?> GetByStudentAndClassTeacherAsync(Guid studentId, Guid classTeacherId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Teacher)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Class)
                .FirstOrDefaultAsync(e => e.IdStudent == studentId && e.IdClassTeacher == classTeacherId);
        }

        public async Task<Enrollment?> GetByStudentIdSingleAsync(Guid studentId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Teacher)
                .Include(e => e.ClassTeacher)
                    .ThenInclude(ct => ct.Class)
                .FirstOrDefaultAsync(e => e.IdStudent == studentId);
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
            var enrollment = await GetByIdAsync(id);
            if (enrollment == null) return false;

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Enrollments.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid classTeacherId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.IdStudent == studentId && e.IdClassTeacher == classTeacherId);
        }

        public async Task<bool> IsStudentEnrolledInAnyClassAsync(Guid studentId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.IdStudent == studentId);
        }
    }
}
