using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using pjatk_apbd.Models;

namespace pjatk_apbd.Controllers
{
  [ApiController]
  [Route("api/test")]
  public class TestController : ControllerBase
  {
    private readonly cw10Context _context;

    public TestController(cw10Context context)
    {
      _context = context;
    }

    [HttpGet()]
    public IActionResult Test()
    {
      _context.Database.OpenConnection();
      return Ok("works");
    }

  }
}

