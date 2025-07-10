using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Students.Entities;

namespace SchoolManagementSystem.Modules.Students.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly DatabaseConfig _context;
        
        public StudentRepository(DatabaseConfig context)
        {
            _context = context;
        }
        
        public async Task<Student> CreateAsync(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }
        
        public async Task<Student?> GetByIdAsync(Guid id)
        {
            return await _context.Students
                .FromSqlRaw("SELECT * FROM \"Students\" WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();
        }
        
        public async Task<List<Student>> GetAllAsync()
        {
            return await _context.Students
                .FromSqlRaw("SELECT * FROM \"Students\"")
                .ToListAsync();
        }
        
        public async Task<Student> UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return student;
        }
        
        public async Task<bool> DeleteAsync(Guid id)
        {
            var student = await _context.Students
                .FromSqlRaw("SELECT * FROM \"Students\" WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();
            if (student == null) return false;
            
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(List<Student> students, int totalCount)> GetAllPaginatedAsync(int page, int size)
        {
            var offset = (page - 1) * size;
            
            // Get total count using CountAsync
            var totalCount = await _context.Students.CountAsync();
            
            // Get paginated data
            var students = await _context.Students
                .FromSqlRaw("SELECT * FROM \"Students\" ORDER BY \"CreatedAt\" DESC OFFSET {0} LIMIT {1}", offset, size)
                .ToListAsync();
                
            return (students, totalCount);
        }

        public async Task<int> GetTotalCountAsync()
        {
            var result = await _context.Database
                .SqlQueryRaw<int>("SELECT COUNT(*) FROM \"Students\"")
                .FirstAsync();
            return result;
        }
    }
}
