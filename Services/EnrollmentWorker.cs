using Microsoft.Extensions.DependencyInjection;

public class EnrollmentWorker
{
    private readonly IServiceScopeFactory _scopeFactory;

    public EnrollmentWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
     public void ProcessBatch()
     {
    
        // Create a short-lived scope
        using var scope = _scopeFactory.CreateScope();

        // Resolve scoped service inside the scope ONLY
        var service = scope.ServiceProvider.GetRequiredService<IEnrollmentService>();

        // Use the service safely
        Console.WriteLine("Processing enrollment batch...");

        //  DEBUG ONLY - to be removed later
    //   service.EnrollAsync(1,1 ).Wait();
    //   service.EnrollAsync(1,1).Wait();
    //   service.GetByIdAsync(1).Wait();       

       
    }

}