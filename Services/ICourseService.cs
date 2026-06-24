using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Entities;
public interface ICourseService
{
    Task<IReadOnlyList<Course>> GetAllAsync();
    Task<Course?> GetByIdAsync(string code);
    Task<Course> CreateAsync(Course course);
    Task<bool> DeleteAsync(string code);
}

