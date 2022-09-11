using Microsoft.AspNetCore.Mvc;
using PollySampleWebApplication.Services.Registries;

namespace PollySampleWebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceHealthController : ControllerBase
{
    private readonly ICircuitBreakerRegistry _registry;

    public ServiceHealthController(ICircuitBreakerRegistry registry)
    {
        _registry = registry;
    }
    [HttpGet]
    public List<ServiceHealth> GetStatus()
    {
        return _registry.GetServicesHealth();
    }
}