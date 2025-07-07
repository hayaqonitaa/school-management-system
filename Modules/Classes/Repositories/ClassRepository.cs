using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Classes.Entities;

namespace SchoolManagementSystem.Modules.Classes.Repositories
{
    public class ClassRepository : IClassRepository
    {
        private readonly DatabaseConfig _context;

        public ClassRepository(DatabaseConfig context)
        {
            _context = context;
        }

        public async Task<List<Class>> GetAllAsync()
        {
            return await _context.Classes.ToListAsync();
        }

        public async Task<Class?> GetByIdAsync(Guid id)
        {
            return await _context.Classes.FindAsync(id);
        }

        public async Task<Class?> GetByKelasAsync(string kelas)
        {
            return await _context.Classes.FirstOrDefaultAsync(c => c.Kelas == kelas);
        }

        public async Task<Class> CreateAsync(Class classEntity)
        {
            _context.Classes.Add(classEntity);
            await _context.SaveChangesAsync();
            return classEntity;
        }

        public async Task<Class> UpdateAsync(Class classEntity)
        {
            _context.Classes.Update(classEntity);
            await _context.SaveChangesAsync();
            return classEntity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var classEntity = await GetByIdAsync(id);
            if (classEntity == null) return false;

            _context.Classes.Remove(classEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Classes.AnyAsync(c => c.Id == id);
        }

        public async Task<List<Class>> GetByTingkatAsync(int tingkat)
        {
            return await _context.Classes.Where(c => c.Tingkat == tingkat).ToListAsync();
        }
    }
}
