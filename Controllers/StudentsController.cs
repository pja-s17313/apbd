using System;
using Microsoft.AspNetCore.Mvc;
using pjatk_apbd.Models;

namespace pjatk_apbd.Controllers
{
  [ApiController]
  [Route("api/students")]

  public class StudentsController : ControllerBase
  {
    [HttpGet]
    public string GetStudents(string orderBy)
    {
      return $"Kowalski, Malejwski, Andrzejewski sortowanie={orderBy}";
    }

    [HttpGet("{id}")]
    public IActionResult GetStudent(int id)
    {
      if (id == 1)
      {
        return Ok("Kowalski");
      }
      else if (id == 2)
      {
        return Ok("Malewski");
      }

      return NotFound("Nie znaleziono studenta");
    }

    [HttpPost]
    public IActionResult CreateStudent(Student student)
    {
      student.IndexNumber = $"s{new Random().Next(1, 20000)}";
      return Ok(student);
    }

    [HttpPut("{id}")]
    public IActionResult EditStudent(int id, Student student)
    {
      // todo: actually edit
      return Ok("aktualizacja dokończona");
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteStudent(int id)
    {
      // todo: actually delete
      return Ok("usuwanie dokończone");
    }
  }
}