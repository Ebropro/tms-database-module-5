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
        
    public async Task<Student?> UpdateAsync(Student student) // PUT Endpoint
{
    _context.Students.Update(student);
    await _context.SaveChangesAsync();
    return student;
}
}