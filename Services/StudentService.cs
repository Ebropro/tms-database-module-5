using TmsApi.Entities;
using TmsApi.Data;
using Microsoft.EntityFrameworkCore;

public class StudentService : IStudentService
{
    private readonly TmsDbContext _context;

    public StudentService(TmsDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Student>> GetAllAsync()
        => await _context.Students.ToListAsync();

    public async Task<Student?> GetByIdAsync(int id)
        => await _context.Students.FindAsync(id);

    public async Task<Student> CreateAsync(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student is null) return false;

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return true;
    }
        
   public async Task<Student?> UpdateAsync(Student student)
{
    var existing = await _context.Students.FindAsync(student.Id);

    if (existing == null)
        return null;

    existing.Name = student.Name;
    existing.Age = student.Age;
    existing.GPA = student.GPA;
    existing.IsActive = student.IsActive;
    existing.RegistrationNumber = student.RegistrationNumber;

     _context.Entry(existing)
        .Property(s => s.Version)
        .OriginalValue = student.Version;

     //existing.Version++;

    await _context.SaveChangesAsync();

    return existing;
}
// =========================================================
    // ✅ Task 1 — Soft-delete
    // Sets IsDeleted = true.  The HasQueryFilter hides this student
    // from every normal query going forward — no code changes needed elsewhere.
    // =========================================================
    public async Task<bool> SoftDeleteAsync(int id)
    {
        // FindAsync / Students.FirstOrDefault respect the query filter,
        // so a soft-deleted student would not be found a second time.
        var student = await _context.Students.FindAsync(id);
        if (student is null) return false;

        student.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    // =========================================================
    // ✅ Task 1 — Admin restore: IgnoreQueryFilters() bypasses HasQueryFilter
    // so soft-deleted rows are visible again.
    // =========================================================
    public async Task<IReadOnlyList<Student>> GetAllIncludingDeletedAsync()
        => await _context.Students
            .IgnoreQueryFilters()   // <-- bypasses !IsDeleted filter
            .ToListAsync();

    public async Task<bool> RestoreAsync(int id)
    {
        // Must use IgnoreQueryFilters() — otherwise EF can't find the soft-deleted row
        var student = await _context.Students
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student is null) return false;

        student.IsDeleted = false;
        await _context.SaveChangesAsync();
        return true;
    }

}


