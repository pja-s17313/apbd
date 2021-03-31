using System;
using System.Collections.Generic;

namespace pjatk_apbd.Models
{
  public partial class Enrollment
  {
    public Enrollment()
    {
      Student = new HashSet<Student>();
    }

    public int IdEnrollment { get; set; }
    public int Semester { get; set; }
    public int IdStudy { get; set; }
    public virtual Studies Studies { get; set; }
    public DateTime StartDate { get; set; }

    public virtual Studies IdStudyNavigation { get; set; }
    public virtual ICollection<Student> Student { get; set; }
  }
}
