using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Students.Entities;
using SchoolManagementSystem.Modules.Teachers.Entities;
using SchoolManagementSystem.Modules.Users.Entities;

public class DatabaseConfig : DbContext
{
    public DatabaseConfig(DbContextOptions<DatabaseConfig> options): base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<User> Users { get; set; }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => (e.Entity is Student || e.Entity is Teacher || e.Entity is User) && (
                e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            if (entityEntry.Entity is Student student)
            {
                student.UpdatedAt = DateTime.UtcNow;
                if (entityEntry.State == EntityState.Added)
                {
                    student.CreatedAt = DateTime.UtcNow;
                }
            }
            else if (entityEntry.Entity is Teacher teacher)
            {
                teacher.UpdatedAt = DateTime.UtcNow;
                if (entityEntry.State == EntityState.Added)
                {
                    teacher.CreatedAt = DateTime.UtcNow;
                }
            }
            else if (entityEntry.Entity is User user)
            {
                user.UpdatedAt = DateTime.UtcNow;
                if (entityEntry.State == EntityState.Added)
                {
                    user.CreatedAt = DateTime.UtcNow;
                }
            }
        }

        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => (e.Entity is Student || e.Entity is Teacher || e.Entity is User) && (
                e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            if (entityEntry.Entity is Student student)
            {
                student.UpdatedAt = DateTime.UtcNow;
                if (entityEntry.State == EntityState.Added)
                {
                    student.CreatedAt = DateTime.UtcNow;
                }
            }
            else if (entityEntry.Entity is Teacher teacher)
            {
                teacher.UpdatedAt = DateTime.UtcNow;
                if (entityEntry.State == EntityState.Added)
                {
                    teacher.CreatedAt = DateTime.UtcNow;
                }
            }
            else if (entityEntry.Entity is User user)
            {
                user.UpdatedAt = DateTime.UtcNow;
                if (entityEntry.State == EntityState.Added)
                {
                    user.CreatedAt = DateTime.UtcNow;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
