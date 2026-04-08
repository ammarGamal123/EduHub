using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProject.Data.Entities
{
    public class DepartmentSubject
    {

        public int DeptID { get;set;}

        public int SubID { get; set; }

        [ForeignKey("DeptID")]
        [InverseProperty(nameof(Department.DepartmentSubjects))]
        public virtual Department Department { get; set; }

        [ForeignKey("SubID")]
        [InverseProperty(nameof(Subject.DepartmentSubjects))]
        public virtual Subject Subject { get; set; }  
    }
}
