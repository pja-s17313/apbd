using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using pjatk_apbd.Models;

namespace pjatk_apbd.Controllers
{
  [ApiController]
  [Route("api/students")]
  public class EnrollmentsController : ControllerBase
  {
    private readonly cw10Context _context;

    public EnrollmentsController(cw10Context context)
    {
      _context = context;
    }

    [HttpPost()]
    public IActionResult CreateEnrollment(CreateEnrollmentDto dto)
    {
      if (dto.IndexNumber == null || dto.FirstName == null || dto.LastName == null || dto.BirthDate == null || dto.Studies == null)
      {
        return BadRequest("Please fill in all required fields");
      }
      DateTime birthDate;
      try
      {
        DateTime.TryParseExact(dto.BirthDate, "dd.MM.yyyy", new CultureInfo("pl-PL"), DateTimeStyles.None, out birthDate);
      }
      catch
      {
        return BadRequest("Birth date is not in a proper format");
      }
      if (_context.Student.Any(s => s.IndexNumber == dto.IndexNumber))
      {
        return BadRequest("Student with that index number already exists");
      }

      var enr = _context.Enrollment.Where(e => e.Studies.Name == dto.Studies && e.Semester == 1).SingleOrDefault();
      if (enr == null)
      {
        enr = new Enrollment
        {
          Semester = 1,
          Studies = _context.Studies.Where(s => s.Name == dto.Studies).SingleOrDefault(),
          StartDate = DateTime.Now
        };
        if (enr.Studies == null)
        {
          return BadRequest("Studies don't exist");
        }
      }

      var student = new Student
      {
        IndexNumber = dto.IndexNumber,
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        BirthDate = birthDate,
        Enrollment = enr,
      };

      _context.SaveChanges();

      return Ok(enr);

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

