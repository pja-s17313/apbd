using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using pjatk_apbd.DAL;
using pjatk_apbd.Models;
using pjatk_apbd;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace pjatk_apbd.Controllers
{
  [ApiController]
  [Route("api/login")]

  public class LoginController : ControllerBase
  {
    private readonly IDbService _dbService;
    private readonly IConfiguration _configuration;

    public LoginController(IConfiguration configuration, IDbService dbService)
    {
      _configuration = configuration;
      _dbService = dbService;
    }

    [HttpPost]
    public IActionResult GenerateAccessToken(LoginReq req)
    {
      if (req.Login == null || req.Password == null)
      {
        return BadRequest("Please fill in all required fields");
      }
      using (var client = new SqlConnection("Server=db-mssql.pjwstk.edu.pl;Database=s17313;User Id=apbds17313;Password=admin;"))
      using (var command = new SqlCommand())
      {
        client.Open();
        command.Connection = client;
        command.CommandText = "SELECT 1 FROM student s WHERE s.indexNumber = @login AND s.password = @password";
        command.Parameters.AddWithValue("login", req.Login);
        command.Parameters.AddWithValue("password", req.Password);

        if (command.ExecuteScalar() == null)
        {
          return BadRequest("Wrong login and/or password");
        }
        client.Close();
      }

      var claims = new[]{
          new Claim(ClaimTypes.NameIdentifier, req.Login),
          new Claim(ClaimTypes.Role, "student")
        };

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSecret"]));
      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
        issuer: "APBD",
        audience: "Students",
        claims: claims,
        expires: DateTime.Now.AddMinutes(30),
        signingCredentials: credentials
      );

      return Ok(new JwtSecurityTokenHandler().WriteToken(token));
    }
  }
}