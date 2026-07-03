using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Entities;
using Microsoft.Extensions.Logging;

public class EnrollmentService : IEnrollmentService
{
    private readonly TmsDbContext _dbContext;
    private readonly ILogger<EnrollmentService> _logger;

    public EnrollmentService(TmsDbContext dbContext, ILogger<EnrollmentService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    // CREATE ENROLLMENT
    public async Task<Enrollment> EnrollAsync(int studentId, int courseId)
    {
        var existing = await _dbContext.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

        if (existing != null)
        {
            _logger.LogWarning("Duplicate enrollment Student {StudentId} Course {CourseId}", studentId, courseId);
            return existing;
        }

        var enrollment = new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            Grade = 0
        };

        _dbContext.Enrollments.Add(enrollment);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Enrolled Student {StudentId} in Course {CourseId}", studentId, courseId);
        return enrollment;
    }

    // GET BY ID
    public async Task<Enrollment?> GetByIdAsync(int id)
        => await _dbContext.Enrollments.FirstOrDefaultAsync(e => e.Id == id);

    // GET ALL
    public async Task<IReadOnlyList<Enrollment>> GetAllAsync()
        => await _dbContext.Enrollments.ToListAsync();

    // DELETE
    public async Task<bool> DeleteAsync(int id)
    {
        var enrollment = await _dbContext.Enrollments.FirstOrDefaultAsync(e => e.Id == id);
        if (enrollment is null)
        {
            _logger.LogWarning("Enrollment {Id} not found", id);
            return false;
        }

        _dbContext.Enrollments.Remove(enrollment);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Deleted enrollment {Id}", id);
        return true;
    }

    // =========================================================
    // ✅ Task 2 — Bulk archive via ExecuteUpdateAsync
    //
    // This issues a SINGLE SQL statement:
    //   UPDATE "Enrollments" SET "IsArchived" = true
    //   WHERE "EnrolledAt" < @cutoff AND "IsArchived" = false
    //
    // No rows are loaded into memory — EF translates the entire
    // expression tree into one server-side UPDATE.
    // =========================================================
    public async Task<int> ArchiveOlderThanAsync(
        DateTime cutoff,
        CancellationToken cancellationToken = default)
    {
        var affected = await _dbContext.Enrollments
            .Where(e => e.EnrolledAt < cutoff && !e.IsArchived)
            .ExecuteUpdateAsync(
                s => s.SetProperty(e => e.IsArchived, true),
                cancellationToken);

        _logger.LogInformation(
            "Archived {Count} enrollments older than {Cutoff:O}", affected, cutoff);

        return affected;
    }
}
