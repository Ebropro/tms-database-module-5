using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Entities;
public class CourseService : ICourseService
{
    private readonly TmsDbContext _dbContext;

    public CourseService(TmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // GET ALL COURSES
    public async Task<IReadOnlyList<Course>> GetAllAsync()
    {
        return await _dbContext.Courses.ToListAsync();
    }

    // GET COURSE BY CODE
    public async Task<Course?> GetByIdAsync(string code)
    {
        return await _dbContext.Courses
            .FirstOrDefaultAsync(c => c.Code == code);
    }

    // CREATE COURSE
    public async Task<Course> CreateAsync(Course course)
    {
        _dbContext.Courses.Add(course);
        await _dbContext.SaveChangesAsync();
        return course;
    }

    // DELETE COURSE
    public async Task<bool> DeleteAsync(string code)
    {
        var course = await _dbContext.Courses
            .FirstOrDefaultAsync(c => c.Code == code);

        if (course is null)
            return false;

        _dbContext.Courses.Remove(course);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}

// DTO

// using Microsoft.EntityFrameworkCore;
// using TmsApi.Data;
// using TmsApi.Entities;
// using TmsApi.Dtos;

// public class CourseService : ICourseService
// {
//     private readonly TmsDbContext _context;

//     public CourseService(TmsDbContext context)
//     {
//         _context = context;
//     }

//     // GET ALL → DTO ONLY (code, title, capacity)
//     public async Task<IReadOnlyList<CourseDto>> GetAllAsync()
//     {
//         return await _context.Courses
//             .Select(c => new CourseDto
//             {
//                 Code = c.Code,
//                 Title = c.Title,
//                 Capacity = c.Capacity
//             })
//             .ToListAsync();
//     }

//     // GET BY ID → DTO
//     public async Task<CourseDto?> GetByIdAsync(string code)
//     {
//         return await _context.Courses
//             .Where(c => c.Code == code)
//             .Select(c => new CourseDto
//             {
//                 Code = c.Code,
//                 Title = c.Title,
//                 Capacity = c.Capacity
//             })
//             .FirstOrDefaultAsync();
//     }

//     // CREATE (still uses entity input)
//     public async Task<CourseDto> CreateAsync(Course course)
//     {
//         _context.Courses.Add(course);
//         await _context.SaveChangesAsync();

//         return new CourseDto
//         {
//             Code = course.Code,
//             Title = course.Title,
//             Capacity = course.Capacity
//         };
//     }

//     // DELETE (no DTO needed)
//     public async Task<bool> DeleteAsync(string code)
//     {
//         var course = await _context.Courses
//             .FirstOrDefaultAsync(c => c.Code == code);

//         if (course is null)
//             return false;

//         _context.Courses.Remove(course);
//         await _context.SaveChangesAsync();

//         return true;
//     }
// }


