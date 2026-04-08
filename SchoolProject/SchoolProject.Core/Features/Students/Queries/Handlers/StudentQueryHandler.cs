using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Bases;
using SchoolProject.Core.Features.Students.Queries.Models;
using SchoolProject.Core.Features.Students.Queries.Responses;
using SchoolProject.Core.Features.Students.Queries.Results;
using SchoolProject.Core.Resources;
using SchoolProject.Core.Wrappers;
using SchoolProject.Data.Entities;
using SchoolProject.Service.Abstracts;
using System.Linq.Expressions;

namespace SchoolProject.Core.Features.Students.Queries.Handlers
{
    public class StudentQueryHandler : 
        ResponseHandler,
        IRequestHandler<GetStudentsListQuery, Response<List<GetStudentsListResponse>>>,
        IRequestHandler<GetStudentByIDQuery, Response<GetStudentByIDResponse>>,
        IRequestHandler<GetStudentPaginatedListQuery, PaginationResult<GetStudentPaginatedListResponse>>
    {
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;

        public StudentQueryHandler(
                                    IStudentService studentService,
                                    IMapper mapper,
                                    IStringLocalizer<SharedResources> stringLocalizer)
                                    : base(stringLocalizer)
        {
            _studentService = studentService;
            _mapper = mapper;
            _stringLocalizer = stringLocalizer;
        }

        public async Task<Response<List<GetStudentsListResponse>>> Handle(GetStudentsListQuery request, CancellationToken cancellationToken)
        {
            var students = await _studentService.GetAllStudentsAsync();
            var studentsMapper = _mapper.Map<List<GetStudentsListResponse>>(students);

            var result = Success(studentsMapper);
            result.Meta = new { Count = studentsMapper.Count() };

            return result;
        }

        public async Task<Response<GetStudentByIDResponse>> Handle(GetStudentByIDQuery request, CancellationToken cancellationToken)
        {
            var student = await _studentService.GetStudentWithIncludingByIDAsync(request.Id);
            if (student == null)
                return NotFound<GetStudentByIDResponse>($"{_stringLocalizer[SharedResourcesKeys.NotFound]} {request.Id}");

            var studentMapper = _mapper.Map<GetStudentByIDResponse>(student);

            return Success(studentMapper);
        }

        public async Task<PaginationResult<GetStudentPaginatedListResponse>> Handle(GetStudentPaginatedListQuery request, CancellationToken cancellationToken)
        {
            // Expression looks like mapping from student to GetStudentPaginatedListResponse
            Expression<Func<Student, GetStudentPaginatedListResponse>> expression
                = stud => new GetStudentPaginatedListResponse(
                stud.StudID,
                stud.Localize(stud.NameAr , stud.NameEn),
                stud.Address,
                // in IQueryable Needs Department to be included
                stud.Department.Localize(stud.Department.NameAr , stud.Department.NameEn)
            );

            var filterQuery = _studentService.FilterStudentPaginatedQuery(
                request.OrderBy, request.Search);

            var paginatedList = await filterQuery.Select(expression)
                               .ToPaginationListAsync(request.PageNumber, request.PageSize);

            var result = paginatedList;
            result.Meta = new { Count = paginatedList.Data.Count() };
            return result;
        }
    }
}
