using TmsApi.Entities;

public interface IEnrollmentService
{
    Task<Enrollment> EnrollAsync(int studentId, int courseId);
    Task<Enrollment?> GetByIdAsync(int id);
    Task<IReadOnlyList<Enrollment>> GetAllAsync();
    Task<bool> DeleteAsync(int id);

    // ✅ Task 2: Bulk-archive enrollments older than cutoff — single SQL UPDATE
    Task<int> ArchiveOlderThanAsync(DateTime cutoff, CancellationToken cancellationToken = default);
}
