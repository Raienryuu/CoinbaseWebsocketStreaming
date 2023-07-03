using Microsoft.AspNetCore.Mvc;
using StreamingWithBackpressure.ResponseModels;

namespace StreamingWithBackpressure.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class SocketController : ControllerBase
    {

        [HttpGet]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        async public void Get()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            
        }
    }
}