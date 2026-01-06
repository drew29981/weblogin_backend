using Microsoft.AspNetCore.Mvc;

namespace wblg.controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("y")]
        public IActionResult Test()
        {
            return Ok(new
            {
                V = 12,
                X = 0,
                S = "drew",
            });

        }
    }
}