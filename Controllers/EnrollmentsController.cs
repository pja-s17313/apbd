using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pjatk_apbd.DAL;
using pjatk_apbd.Models;

namespace pjatk_apbd.Controllers
{
  [ApiController]
  [Route("api/enrollments")]

  public class EnrollmentsController : ControllerBase
  {
    private readonly IDbService _dbService;

    public EnrollmentsController(IDbService dbService)
    {
      _dbService = dbService;
    }

    [Authorize(Roles = "student")]
    [HttpPost]
    public IActionResult CreateEnrollment(Enrollment enr)
    {
      if (enr.IndexNumber == null || enr.FirstName == null || enr.LastName == null || enr.BirthDate == null || enr.Studies == null)
      {
        return BadRequest("Please fill in all required fields");
      }
      using (var client = new SqlConnection("Server=db-mssql.pjwstk.edu.pl;Database=s17313;User Id=apbds17313;Password=admin;"))
      using (var command = new SqlCommand())
      {
        client.Open();
        var transaction = client.BeginTransaction();

        command.Connection = client;
        command.Transaction = transaction;

        command.CommandText = "SELECT 1 FROM Student WHERE IndexNumber = @index";
        command.Parameters.AddWithValue("index", enr.IndexNumber);
        if (command.ExecuteScalar() != null)
        {
          return BadRequest("Student with that index number already exists");
        }

        command.CommandText = "SELECT e.IdEnrollment, s.IdStudy FROM Enrollment e RIGHT JOIN Studies s ON e.IdStudy = s.IdStudy WHERE s.Name = @studyName AND e.Semester = 1";
        command.Parameters.AddWithValue("studyName", enr.Studies);
        command.Transaction = transaction;

        var reader = command.ExecuteReader();
        reader.Read();
        if (reader == null)
        {
          return BadRequest("Studies don't exist");
        }

        int IdEnrollment;
        if (reader["IdEnrollment"] != null)
        {
          IdEnrollment = (int)reader["IdEnrollment"];
        }
        else
        {
          command.CommandText = "INSERT INTO Enrollment (Semester, IdStudy, StartDate) output INSERTED.ID VALUES (1, @idStudy, @startDate)";
          command.Parameters.AddWithValue("idStudy", reader["IdStudy"]);
          command.Parameters.AddWithValue("startDate", DateTime.Today);
          IdEnrollment = (int)command.ExecuteScalar();
        }

        command.CommandText = "INSERT INTO Student (IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES (@index, @firstName, @lastName, @birthDate, @enrId)";
        command.Parameters.AddWithValue("index", enr.IndexNumber);
        command.Parameters.AddWithValue("firstName", enr.FirstName);
        command.Parameters.AddWithValue("lastName", enr.LastName);
        command.Parameters.AddWithValue("birthDate", DateTime.ParseExact(enr.BirthDate, "dd.MM.yyyy", null));
        command.ExecuteNonQuery();

        transaction.Commit();

        client.Close();
        return Ok(enr);
      }
    }
  }
}