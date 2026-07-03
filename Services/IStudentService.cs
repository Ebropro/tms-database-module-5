using TmsApi.Entities;

public interface IStudentService
{
    Task<IReadOnlyList<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(int id);
    Task<Student> CreateAsync(Student student);
    Task<bool> DeleteAsync(int id);
    Task<Student?> UpdateAsync(Student student);

    // ✅ Task 1: Soft-delete (sets IsDeleted = true, student hidden from normal queries)
    Task<bool> SoftDeleteAsync(int id);

    // ✅ Task 1: Admin restore — bypasses HasQueryFilter via IgnoreQueryFilters()
    Task<IReadOnlyList<Student>> GetAllIncludingDeletedAsync();
    Task<bool> RestoreAsync(int id);
}
