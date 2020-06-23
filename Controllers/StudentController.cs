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
  [Route("api/student")]
  public class StudentController : ControllerBase
  {
    private readonly cw10Context _context;

    public StudentController(cw10Context context)
    {
      _context = context;
    }

    [HttpGet()]
    public IActionResult GetStudents()
    {
      return Ok(_context.Student.ToList());
    }

    [HttpGet("{index}")]
    public IActionResult GetStudent(string index)
    {
      var res = _context.Student.Where(s => s.IndexNumber == index).SingleOrDefault();
      if (res == null)
      {
        return NotFound("student not found");
      }
      return Ok(res);
    }

    [HttpPut("{index}")]
    public IActionResult EditStudent(string index, Student newData)
    {
      if (!_context.Student.Any(s => s.IndexNumber == index))
      {
        return NotFound("student not found");
      }

      newData.IndexNumber = index;
      _context.Student.Update(newData);

      _context.SaveChanges();

      return Ok(newData);
    }

    [HttpDelete("{index}")]
    public IActionResult RemoveStudent(string index)
    {
      if (!_context.Student.Any(s => s.IndexNumber == index))
      {
        return NotFound("student not found");
      }

      _context.Student.Remove(new Student { IndexNumber = index });

      _context.SaveChanges();

      return Ok("student removed");
    }

  }
}

