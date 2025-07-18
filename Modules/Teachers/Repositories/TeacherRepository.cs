using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Teachers.Entities;

namespace SchoolManagementSystem.Modules.Teachers.Repositories
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly DatabaseConfig _context;

        public TeacherRepository(DatabaseConfig context)
        {
            _context = context;
        }

        public async Task<Teacher> CreateAsync(Teacher teacher)
        {
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();
            return teacher;
        }

        public async Task<Teacher?> GetByIdAsync(Guid id)
        {
            return await _context.Teachers
                .FromSqlRaw("SELECT * FROM \"Teachers\" WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Teacher>> GetAllAsync()
        {
            return await _context.Teachers
                .FromSqlRaw("SELECT * FROM \"Teachers\"")
                .ToListAsync();
        }

        public async Task<(List<Teacher> teachers, int totalCount)> GetAllPaginatedAsync(int page, int pageSize)
        {
            var offset = (page - 1) * pageSize;
            
            // Get total count
            var totalCount = await _context.Teachers
                .FromSqlRaw("SELECT COUNT(*) FROM \"Teachers\"")
                .CountAsync();

            // Get paginated data
            var teachers = await _context.Teachers
                .FromSqlRaw("SELECT * FROM \"Teachers\" ORDER BY \"CreatedAt\" DESC OFFSET {0} LIMIT {1}", offset, pageSize)
                .ToListAsync();

            return (teachers, totalCount);
        }

        public async Task<Teacher> UpdateAsync(Teacher teacher)
        {
            _context.Teachers.Update(teacher);
            await _context.SaveChangesAsync();
            return teacher;
        }

        public async Task<bool> DeleteAsync(Guid id)
        { 
            var teacher = await _context.Teachers
                .FromSqlRaw("SELECT * FROM \"Teachers\" WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();
            if (teacher == null) return false;

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}