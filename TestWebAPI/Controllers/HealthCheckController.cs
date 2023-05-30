using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entity;

namespace TestWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet()]
        public ActionResult<IQueryable<TodoItem>> Get()
        {
            //Console.WriteLine("Health Check by Jiang-"+DateTime.Now.ToLongDateString());
            return Ok("OK");
        }

    }
}
