using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Students.Entities;
using SchoolManagementSystem.Modules.Teachers.Entities;
using SchoolManagementSystem.Modules.Users.Entities;
using SchoolManagementSystem.Modules.Admins.Entities;
using SchoolManagementSystem.Modules.Classes.Entities;

public class DatabaseConfig : DbContext
{
    public DatabaseConfig(DbContextOptions<DatabaseConfig> options): base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<ClassTeacher> ClassTeachers { get; set; }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => (e.Entity is Student || e.Entity is Teacher || e.Entity is User || e.Entity is Admin || e.Entity is Class || e.Entity is ClassTeacher) && (
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
            else if (entityEntry.Entity is Admin admin)
            {
                admin.UpdatedAt = DateTime.UtcNow;
                if (entityEntry.State == EntityState.Added)
                {
                    admin.CreatedAt = DateTime.UtcNow;
                }
            }
        }

        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => (e.Entity is Student || e.Entity is Teacher || e.Entity is User || e.Entity is Admin) && (
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
            else if (entityEntry.Entity is Admin admin)
            {
                admin.UpdatedAt = DateTime.UtcNow;
                if (entityEntry.State == EntityState.Added)
                {
                    admin.CreatedAt = DateTime.UtcNow;
                }
            }
            else if (entityEntry.Entity is Class classEntity)
            {
                classEntity.UpdatedAt = DateTime.UtcNow;
                if (entityEntry.State == EntityState.Added)
                {
                    classEntity.CreatedAt = DateTime.UtcNow;
                }
            }
            else if (entityEntry.Entity is ClassTeacher classTeacher)
            {
                classTeacher.UpdatedAt = DateTime.UtcNow;
                if (entityEntry.State == EntityState.Added)
                {
                    classTeacher.CreatedAt = DateTime.UtcNow;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ClassTeacher>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IdTeacher).IsRequired();
            entity.Property(e => e.IdClass).IsRequired();
            entity.Property(e => e.Tahun).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            
            // Configure relationships
            entity.HasOne(ct => ct.Teacher)
                .WithMany()
                .HasForeignKey(ct => ct.IdTeacher)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(ct => ct.Class)
                .WithMany()
                .HasForeignKey(ct => ct.IdClass)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
