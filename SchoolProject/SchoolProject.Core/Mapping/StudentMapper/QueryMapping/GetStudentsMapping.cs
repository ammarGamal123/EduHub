using SchoolProject.Core.Features.Students.Queries.Responses;
using SchoolProject.Core.Features.Students.Queries.Results;
using SchoolProject.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProject.Core.Mapping.StudentMapper
{
    public partial class StudentProfile
    {
        public void GetStudentsMapping()
        {
            
            CreateMap<Student, GetStudentsListResponse>()
                .ForMember(dest => dest.DepartmentName,
                           opt => opt.MapFrom(src => src.Department.Localize(src.Department.NameAr 
                                                                           , src.Department.NameEn)))
                .ForMember(dest => dest.Name , 
                           opt => opt.MapFrom(src => src.Localize(src.NameAr , src.NameEn)))
                .ReverseMap();

        }
    }
}
