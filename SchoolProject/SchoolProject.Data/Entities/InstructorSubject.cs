using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProject.Data.Entities
{
    public class InstructorSubject
    {
        public int InstructorID { get; set; }
        public int SubjectID { get; set; }


        [ForeignKey("InstructorID")]
        [InverseProperty(nameof(Instructor.InstructorSubjects))]
        public Instructor Instructor { get; set; }

        [ForeignKey("SubjectID")]
        [InverseProperty(nameof(Subject.InstructorSubjects))]
        public Subject Subject { get; set; }    

    }
} 