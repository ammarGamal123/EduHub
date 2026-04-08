using SchoolProject.Data.Commons;
using SchoolProject.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Instructor : GeneralLocalizableEntity
{
    public Instructor()
    {
        Instructors = new HashSet<Instructor>();
        InstructorSubjects = new HashSet<InstructorSubject>();
    }

    [Key]
    public int InstID { get; set; }

    public string ENameAr { get; set; }

    public string ENameEn { get; set; }

    public string Address { get; set; }

    public string Position { get; set; }

    public int SupervisorID { get; set; }

    public decimal Salary { get; set; }

    public int DepartmentID { get; set; }

    [ForeignKey(nameof(DepartmentID))]
    [InverseProperty(nameof(Department.Instructors))]
    public Department Department { get; set; }

    [InverseProperty(nameof(Department.Instructor))] // Corrected this line
    public Department DepartmentManager { get; set; } // Fixed the typo here

    [ForeignKey(nameof(SupervisorID))]
    [InverseProperty(nameof(Instructor.Instructors))]
    public Instructor Supervisor { get; set; }

    [InverseProperty(nameof(Instructor.Supervisor))]
    public virtual ICollection<Instructor> Instructors { get; set; }

    [InverseProperty(nameof(InstructorSubject.Instructor))]
    public virtual ICollection<InstructorSubject> InstructorSubjects { get; set; }
}
