using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using pjatk_apbd.DAL;
using pjatk_apbd.Models;

namespace pjatk_apbd.Controllers
{
  [ApiController]
  [Route("api/students")]

  public class StudentsController : ControllerBase
  {
    private readonly IDbService _dbService;

    public StudentsController(IDbService dbService)
    {
      _dbService = dbService;
    }

    [HttpGet]
    public IActionResult GetStudents()
    {
      var result = new List<Student>();
      using (var client = new SqlConnection("Server=db-mssql.pjwstk.edu.pl;Database=s17313;User Id=apbds17313;Password=admin;"))
      using (var command = new SqlCommand())
      {
        command.Connection = client;
        command.CommandText = "SELECT s.IndexNumber, s.FirstName, s.LastName, e.Semester, st.Name FROM Student s LEFT JOIN Enrollment e ON s.IdEnrollment = e.IdEnrollment LEFT JOIN Studies st ON e.IdStudy = st.IdStudy";

        client.Open();
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
          var st = new Student();

          st.FirstName = reader["FirstName"].ToString();
          st.LastName = reader["LastName"].ToString();
          st.IndexNumber = reader["IndexNumber"].ToString();
          st.Semester = reader.GetInt32(reader.GetOrdinal("Semester"));
          st.Course = reader["Name"].ToString();
          result.Add(st);
        }
      }
      return Ok(result);
    }

    [HttpGet("{indexNumber}")]
    public IActionResult GetStudent(string indexNumber)
    {
      using (var client = new SqlConnection("Server=db-mssql.pjwstk.edu.pl;Database=s17313;User Id=apbds17313;Password=admin;"))
      using (var command = new SqlCommand())
      {
        command.Connection = client;
        // PRONE TO SQL INJECTION
        // https://localhost:5001/api/students/2; DROP TABLE Student;
        command.CommandText = "SELECT s.IndexNumber, s.FirstName, s.LastName, e.Semester, st.Name FROM Student s LEFT JOIN Enrollment e ON s.IdEnrollment = e.IdEnrollment LEFT JOIN Studies st ON e.IdStudy = st.IdStudy WHERE s.IndexNumber = " + indexNumber;

        client.Open();
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
          var st = new Student();

          st.FirstName = reader["FirstName"].ToString();
          st.LastName = reader["LastName"].ToString();
          st.IndexNumber = reader["IndexNumber"].ToString();
          st.Semester = reader.GetInt32(reader.GetOrdinal("Semester"));
          st.Course = reader["Name"].ToString();

          return Ok(st);
        }
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