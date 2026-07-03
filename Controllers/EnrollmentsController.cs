using TmsApi.Entities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/enrollments")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _enrollmentService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var record = await _enrollmentService.GetByIdAsync(id);
        return record is not null ? Ok(record) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateEnrollmentRequest request)
    {
        var record = await _enrollmentService.EnrollAsync(
            request.StudentId, request.CourseCode);

        return CreatedAtAction(nameof(GetById), new { id = record.Id }, record);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
        => await _enrollmentService.DeleteAsync(id) ? NoContent() : NotFound();

    // =========================================================
    // ✅ Task 2 — Bulk archive endpoint
    //
    // POST /api/enrollments/archive?cutoffYear=2023
    //
    // Internally calls ExecuteUpdateAsync → single SQL UPDATE:
    // =========================================================
    [HttpPost("archive")]
    public async Task<IActionResult> ArchiveOld(
        [FromQuery] int cutoffYear,
        CancellationToken cancellationToken)
    {
        // Everything enrolled before Jan 1 of cutoffYear is archived
        var cutoff = new DateTime(cutoffYear, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var affected = await _enrollmentService.ArchiveOlderThanAsync(cutoff, cancellationToken);

        return Ok(new
        {
            archivedCount = affected,
            cutoff = cutoff.ToString("O"),
            message = $"Issued a single UPDATE — {affected} enrollment(s) marked IsArchived = true."
        });
    }
}

public record CreateEnrollmentRequest(int StudentId, int CourseCode);
