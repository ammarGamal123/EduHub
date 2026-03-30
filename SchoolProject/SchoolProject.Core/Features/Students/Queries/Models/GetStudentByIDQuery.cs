using MediatR;
using SchoolProject.Core.Bases;
using SchoolProject.Core.Features.Students.Queries.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProject.Core.Features.Students.Queries.Models
{
    public class GetStudentByIDQuery : IRequest<Response<GetStudentByIDResponse>>
    {
        public int Id { get; set; }

        public GetStudentByIDQuery(int id)
        {
            Id = id;  
        }
    }
}
