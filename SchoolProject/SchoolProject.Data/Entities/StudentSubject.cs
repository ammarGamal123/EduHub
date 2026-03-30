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
    public class StudentSubject 
    {



        public int StudID { get; set; }
        public int SubID { get; set; }

        public int? Grade { get; set; }

        [ForeignKey("StudID")]
        [InverseProperty(nameof(Student.StudentSubjects))]
        public Student Student { get; set; }

        [InverseProperty(nameof(Subject.StudentSubjects))]
        public Subject Subject { get; set; }

    }
}
