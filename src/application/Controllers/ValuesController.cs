using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet("open")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("admin")]
        [Authorize("admin")]
        public ActionResult<IEnumerable<string>> Admin()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("user")]
        [Authorize("user")]
        public ActionResult<IEnumerable<string>> AllUsers()
        {
            return new string[] { "value1", "value2" };
        }

    }
}
