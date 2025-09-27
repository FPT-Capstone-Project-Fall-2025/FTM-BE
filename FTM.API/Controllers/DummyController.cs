using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FTM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DummyController : ControllerBase
    {
        public DummyController() { }

        [HttpGet("/")]
        [AllowAnonymous]
        public async Task<String> GetDummy()
        {
            return "This is dummy api using get method";
        }
    }
}
