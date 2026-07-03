using TmsApi.Entities;
using TmsApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;

    public StudentsController(IStudentService service)
    {
        _service = service;
    }

    // ── Normal list: soft-deleted students are INVISIBLE (HasQueryFilter applied)
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var student = await _service.GetByIdAsync(id);
        return student is null ? NotFound() : Ok(student);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Student student)
    {
        var created = await _service.CreateAsync(student);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Student student)
    {
        if (id != student.Id)
            return BadRequest("ID mismatch");

        try
        {
            var updated = await _service.UpdateAsync(student);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new
            {
                message = "The record was updated by another user. Please reload and try again."
            });
        }
    }

    // Hard-delete (permanent row removal)
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
        => await _service.DeleteAsync(id) ? NoContent() : NotFound();

    // Task 1 — Soft-delete endpoint
    // Marks the student as IsDeleted = true.
    
  
    [HttpDelete("{id}/soft")]
    public async Task<IActionResult> SoftDelete(int id)
        => await _service.SoftDeleteAsync(id) ? NoContent() : NotFound();

    // Task 1 — Admin: list ALL students including soft-deleted
    
    [HttpGet("admin/all")]
    public async Task<IActionResult> GetAllIncludingDeleted()
        => Ok(await _service.GetAllIncludingDeletedAsync());

    
    // Task 1 — Admin: restore a soft-deleted student
    // Uses IgnoreQueryFilters() to find the row, then clears IsDeleted.
  
    [HttpPost("{id}/restore")]
    public async Task<IActionResult> Restore(int id)
        => await _service.RestoreAsync(id) ? Ok() : NotFound();
}
