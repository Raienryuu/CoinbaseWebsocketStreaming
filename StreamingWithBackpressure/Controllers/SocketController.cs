using Microsoft.AspNetCore.Mvc;
using StreamingWithBackpressure.ResponseModels;

namespace StreamingWithBackpressure.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class SocketController : ControllerBase
    {

        [HttpGet]
        async public void Get()
        {
            
        }
    }
}