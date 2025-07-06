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
            return await _context.Students.FindAsync(id);
        }
        
        public async Task<Student?> GetByNISNAsync(string nisn)
        {
            return await _context.Students.FirstOrDefaultAsync(s => s.NISN == nisn);
        }
        
        public async Task<Student?> GetByEmailAsync(string email)
        {
            return await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
        }
        
        public async Task<List<Student>> GetAllAsync()
        {
            return await _context.Students.ToListAsync();
        }
        
        public async Task<Student> UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return student;
        }
        
        public async Task<bool> DeleteAsync(Guid id)
        {
            var student = await GetByIdAsync(id);
            if (student == null) return false;
            
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
