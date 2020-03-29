using System.Collections.Generic;
using pjatk_apbd.Models;

namespace pjatk_apbd.DAL
{
  public interface IDbService
  {
    public IEnumerable<Student> GetStudents();
  }
}