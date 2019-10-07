using Microsoft.AspNetCore.Mvc;

namespace Test.API.Controllers
{
    /// <summary>
    /// Main Controller
    /// </summary>
    [Route("api/[controller]")]
    public class MainController : Controller
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