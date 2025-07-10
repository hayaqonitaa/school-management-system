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
            return await _context.Classes
                .FromSqlRaw("SELECT * FROM \"Classes\"")
                .ToListAsync();
        }

        public async Task<(List<Class> classes, int totalCount)> GetAllPaginatedAsync(int page, int pageSize)
        {
            var offset = (page - 1) * pageSize;
            
            // Get total count
            var totalCount = await _context.Classes
                .FromSqlRaw("SELECT COUNT(*) FROM \"Classes\"")
                .CountAsync();

            // Get paginated data
            var classes = await _context.Classes
                .FromSqlRaw("SELECT * FROM \"Classes\" ORDER BY \"CreatedAt\" DESC OFFSET {0} LIMIT {1}", offset, pageSize)
                .ToListAsync();

            return (classes, totalCount);
        }

        public async Task<Class?> GetByIdAsync(Guid id)
        {
            return await _context.Classes
                .FromSqlRaw("SELECT * FROM \"Classes\" WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();
        }

        public async Task<Class?> GetByKelasAsync(string kelas)
        {
            return await _context.Classes
                .FromSqlRaw("SELECT * FROM \"Classes\" WHERE \"Kelas\" = {0}", kelas)
                .FirstOrDefaultAsync();
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
            var classEntity = await _context.Classes
                .FromSqlRaw("SELECT * FROM \"Classes\" WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();
            if (classEntity == null) return false;

            _context.Classes.Remove(classEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            var result = await _context.Database
                .SqlQueryRaw<int>("SELECT COUNT(*) FROM \"Classes\" WHERE \"Id\" = {0}", id)
                .FirstAsync();
            return result > 0;
        }

        public async Task<List<Class>> GetByTingkatAsync(int tingkat)
        {
            return await _context.Classes
                .FromSqlRaw("SELECT * FROM \"Classes\" WHERE \"Tingkat\" = {0}", tingkat)
                .ToListAsync();
        }
    }
}
