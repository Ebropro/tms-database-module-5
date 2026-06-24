using TmsApi.Entities;
using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _service;

    public CoursesController(ICourseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> GetById(string code)
    {
        var course = await _service.GetByIdAsync(code);

        return course is null
            ? NotFound()
            : Ok(course);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Course course)
    {
        var created = await _service.CreateAsync(course);

        return CreatedAtAction(
            nameof(GetById),
            new { code = created.Code },
            created);
    }

    [HttpDelete("{code}")]
    public async Task<IActionResult> Delete(string code)
    {
        return await _service.DeleteAsync(code)
            ? NoContent()
            : NotFound();
    }
}