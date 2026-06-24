using TmsApi.Entities;
public interface IStudentService
{
    Task<IReadOnlyList<Student>> GetAllAsync();
    
    Task<Student?> GetByIdAsync(int id);
    Task<Student> CreateAsync(Student student);
    Task<bool> DeleteAsync(int id);

// Put endpoint
    Task<Student?> UpdateAsync(Student student);
}
