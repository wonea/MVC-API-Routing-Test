using Microsoft.AspNetCore.Mvc;

namespace Test.API.Controllers
{
    /// <summary>
    /// Main Controller
    /// </summary>
    [Route(""), ApiController]
    public class MainController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Hello!";
        }

        [HttpPost]
        public string Post(string test)
        {
            return "Hello!";
        }
    }
}