using TmsApi.Entities;
using TmsApi.Data;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;

    public StudentsController(IStudentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var student = await _service.GetByIdAsync(id);

        return student is null
            ? NotFound()
            : Ok(student);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Student student)
    {
        var created = await _service.CreateAsync(student);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        return await _service.DeleteAsync(id)
            ? NoContent()
            : NotFound();
    }

    // Add PUT endpoints 
    [HttpPut("{id}")]
public async Task<IActionResult> Update(int id, [FromBody] Student student)
{
    var existing = await _service.GetByIdAsync(id);

    if (existing == null)
        return NotFound();

    student.Id = id;

    var updated = await _service.UpdateAsync(student); 

    return Ok(updated);
}
}

