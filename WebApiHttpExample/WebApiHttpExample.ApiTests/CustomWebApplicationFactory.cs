using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WebApiHttpExample.Repositories;

namespace WebApiHttpExample.ApiTests;

// This class sets up the in-memory test server for your API.
// It inherits from WebApplicationFactory<TEntryPoint>, where TEntryPoint is
// typically your API's Startup class.
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IUsersRepository> MockUsersRepository { get; private set; }

    public CustomWebApplicationFactory()
    {
        MockUsersRepository = new Mock<IUsersRepository>();
    }

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove any existing registration of IUsersRepository from the service collection.
            // This ensures our mock replaces the real implementation.
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IUsersRepository));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Register the mock instance as a singleton or scoped service.
            // Using Singleton here ensures the same mock instance is used across all tests
            // that use this factory, allowing for consistent setup and verification.
            services.AddSingleton(MockUsersRepository.Object);
        });
    }
}
