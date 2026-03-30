using SchoolProject.Data.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProject.Data.Entities
{
    public class Subject : GeneralLocalizableEntity
    {
        public Subject()
        {
            StudentSubjects = new HashSet<StudentSubject>();
            DepartmentSubjects = new HashSet<DepartmentSubject>();
            InstructorSubjects = new HashSet<InstructorSubject>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubID { get; set; }

        [StringLength(500)]
        public string SubjectNameAr { get; set; }

        [StringLength(500)]
        public string SubjectNameEn { get; set; }


        public DateTime Period { get; set; }

        [InverseProperty(nameof(StudentSubject.Subject))]
        public virtual ICollection<StudentSubject> StudentSubjects { get; set; }

        [InverseProperty(nameof(DepartmentSubject.Subject))]
        public virtual ICollection<DepartmentSubject> DepartmentSubjects { get; set; }

        [InverseProperty(nameof(InstructorSubject.Subject))]
        public virtual ICollection<InstructorSubject> InstructorSubjects { get; set; }

        
    }
}
