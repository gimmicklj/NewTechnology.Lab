using Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.Models;
using Microsoft.Extensions.Options;

namespace AuthServer.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {

        [HttpGet()]
        public ActionResult<IQueryable<User>> Get()
        {
            return Ok("OK");
        }

    }
}
