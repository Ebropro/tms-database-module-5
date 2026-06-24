
// using Microsoft.Extensions.Logging;

// public interface IEnrollmentService
// {
//     Task<EnrollmentRecord> EnrollAsync(string studentId, string courseCode);
//     Task<EnrollmentRecord?> GetByIdAsync(string id);
//     Task<IReadOnlyList<EnrollmentRecord>> GetAllAsync();
//     Task<bool> DeleteAsync(string id);
// }

// public class EnrollmentService : IEnrollmentService
// {
//     private readonly Dictionary<string, EnrollmentRecord> _store = new();
//     private readonly ILogger<EnrollmentService> _logger;

//     public EnrollmentService(ILogger<EnrollmentService> logger)
//     {
//         _logger = logger;
//     }

//     public Task<EnrollmentRecord> EnrollAsync(string studentId, string courseCode)
//     {
//         var existing = _store.Values.FirstOrDefault(e =>
//             e.StudentId == studentId && e.CourseCode == courseCode);

//         if (existing is not null)
//         {
//             _logger.LogWarning(
//                 "Duplicate enrollment attempt {StudentId} already in {CourseCode} (record {EnrollmentId})",
//                 studentId, courseCode, existing.Id);

//             return Task.FromResult(existing);
//         }

//         var id = Guid.NewGuid().ToString("N")[..8];

//         var record = new EnrollmentRecord(
//             id,
//             studentId,
//             courseCode,
//             DateTime.UtcNow);

//         _store[id] = record;

//         _logger.LogInformation(
//             "Enrolled {StudentId} in {CourseCode} record {EnrollmentId}",
//             studentId, courseCode, id);

//         return Task.FromResult(record);
//     }

//     public Task<EnrollmentRecord?> GetByIdAsync(string id)
//     {
//         _store.TryGetValue(id, out var record);

//         if (record is null)
//         {
//             _logger.LogWarning("Enrollment {EnrollmentId} not found", id);
//         }

//         return Task.FromResult(record);
//     }

//     public Task<IReadOnlyList<EnrollmentRecord>> GetAllAsync()
//     {
//         IReadOnlyList<EnrollmentRecord> all = _store.Values.ToList();
//         return Task.FromResult(all);
//     }

//     public Task<bool> DeleteAsync(string id)
//     {
//         var removed = _store.Remove(id);

//         if (removed)
//         {
//             _logger.LogInformation("Deleted enrollment {EnrollmentId}", id);
//         }
//         else
//         {
//             _logger.LogWarning("Delete failed enrollment {EnrollmentId} not found", id);
//         }

//         return Task.FromResult(removed);
//     }
// }


// public record EnrollmentRecord(
//     string Id,
//     string StudentId,
//     string CourseCode,
//     DateTime EnrolledAt);



// public class TmsDatabaseException : Exception
// {
//     public TmsDatabaseException(string message) : base(message)
//     {
//     }
// }

using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Entities;
using Microsoft.Extensions.Logging;
// DBNull Backed
// ================================================
public class EnrollmentService : IEnrollmentService
{
    private readonly TmsDbContext _dbContext;
    private readonly ILogger<EnrollmentService> _logger;

    public EnrollmentService(
        TmsDbContext dbContext,
        ILogger<EnrollmentService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    // CREATE ENROLLMENT
    public async Task<Enrollment> EnrollAsync(int studentId, int courseId)
    {
        var existing = await _dbContext.Enrollments
            .FirstOrDefaultAsync(e =>
                e.StudentId == studentId &&
                e.CourseId == courseId);

        if (existing != null)
        {
            _logger.LogWarning(
                "Duplicate enrollment Student {StudentId} Course {CourseId}",
                studentId, courseId);

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

        _logger.LogInformation(
            "Enrolled Student {StudentId} in Course {CourseId}",
            studentId, courseId);

        return enrollment;
    }

    // GET BY ID
    public async Task<Enrollment?> GetByIdAsync(int id)
    {
        return await _dbContext.Enrollments
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    // GET ALL
    public async Task<IReadOnlyList<Enrollment>> GetAllAsync()
    {
        return await _dbContext.Enrollments.ToListAsync();
    }

    // DELETE
    public async Task<bool> DeleteAsync(int id)
    {
        var enrollment = await _dbContext.Enrollments
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enrollment == null)
        {
            _logger.LogWarning("Enrollment {Id} not found", id);
            return false;
        }

        _dbContext.Enrollments.Remove(enrollment);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Deleted enrollment {Id}", id);

        return true;
    }
}