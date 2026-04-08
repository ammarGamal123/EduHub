using MediatR;
using SchoolProject.Core.Bases;
using SchoolProject.Core.Features.Students.Queries.Responses;
using SchoolProject.Core.Wrappers;
using SchoolProject.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProject.Core.Features.Students.Queries.Models
{
    public class GetStudentPaginatedListQuery :
                 IRequest<PaginationResult<GetStudentPaginatedListResponse>>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public StudentOrderingEnum OrderBy { get; set; }

        public string? Search { get; set; }

    }
}
