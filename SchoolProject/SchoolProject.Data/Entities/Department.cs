using SchoolProject.Data.Commons;
using SchoolProject.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public partial class Department : GeneralLocalizableEntity
{
    public Department()
    {
        Students = new HashSet<Student>();
        DepartmentSubjects = new HashSet<DepartmentSubject>();
    }

    [Key]
    public int DeptID { get; set; }

    [StringLength(300)]
    public string NameEn { get; set; }

    [StringLength(300)]
    public string NameAr { get; set; }

    public int InstructorManager { get; set; }

    [InverseProperty(nameof(Student.Department))]
    public virtual ICollection<Student> Students { get; set; }

    [InverseProperty(nameof(DepartmentSubject.Department))]
    public virtual ICollection<DepartmentSubject> DepartmentSubjects { get; set; }

    [InverseProperty(nameof(Instructor.Department))]
    public ICollection<Instructor> Instructors { get; set; }

    [ForeignKey("InstructorManager")]
    [InverseProperty(nameof(Instructor.DepartmentManager))] // Corrected this line
    public Instructor Instructor { get; set; }
}
