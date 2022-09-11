using Microsoft.AspNetCore.Mvc;
using PollySampleWebApplication.Services;

namespace PollySampleWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly IList<IService> _services;
        public ServicesController(IList<IService> services)
        {
            _services = services;
        }

        [HttpGet]
        public async Task<List<string>> Get()
        {
            var tasks = _services.Select(a => a.Get()).ToList();
            await Task.WhenAll(tasks).ContinueWith(a=>
            {
                if (a.IsFaulted)
                {
                    Console.WriteLine("Error");
                }

            });
            
            return tasks.Where(a=>a.IsCompletedSuccessfully).Select(a => a.Result).ToList();
        }

    }
}