using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Bases;
using SchoolProject.Core.Features.Students.Commands.Models;
using SchoolProject.Core.Resources;
using SchoolProject.Data.Entities;
using SchoolProject.Service.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProject.Core.Features.Students.Commands.Handlers
{
    public class StudentCommandHandler :
                 ResponseHandler,
                 IRequestHandler<CreateStudentCommand , Response<string>> , 
                 IRequestHandler<UpdateStudentCommand , Response<string>> , 
                 IRequestHandler<DeleteStudentCommand , Response<string>>

    {
        #region Fields
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors

        public StudentCommandHandler(
                                     IStudentService studentService,
                                     IMapper mapper,
                                     IStringLocalizer<SharedResources> stringLocalizer)
                                     : base(stringLocalizer)
        {
            _studentService = studentService;
            _mapper = mapper;
            _stringLocalizer = stringLocalizer;
        }
        #endregion

        #region Handle Functions

        public async Task<Response<string>> Handle(CreateStudentCommand request,
                                             CancellationToken cancellationToken)
        {
            // Mapping from request to student
            var student = _mapper.Map<Student>(request);
            // Add
            var result = await _studentService.CreateStudentAsync(student);

            // Check the response coming from _studentService
            if (result == "")
                return UnprocessableEntity<string>("");

            // return response
            else if (result == "Added Successfully")
                return Created<string>("");
            else
                return BadRequest<string>("");
        }

       
        public async Task<Response<string>> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
        {
            // Check if the student with the provided ID exists
            var student = await _studentService.GetStudentByIDAsync(request.StudID);
            if (student == null)
            {
                return NotFound<string>("");
            }

            // Check if the new name already exists for another student
            bool isNameExists = await _studentService.IsNameExistsExcludeSelf(request.Name, request.StudID);
            if (isNameExists)
            {
                return BadRequest<string>("");
            }

            // Map the request data to the student entity
            var mappedStudent = _mapper.Map(request , student);

            // Call the service to update the student
            var result = await _studentService.UpdateStudentAsync(mappedStudent);

            if (result != "Success")
            {
                return BadRequest<string>("");
            }

            return Created<string>("");
        }

        public async Task<Response<string>> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentService.GetStudentByIDAsync(request.StudID);

            if (student == null)
                return NotFound<string>("");

            var result = await _studentService.DeleteStudentAsync(student);

            if (result != $"Success")
                return BadRequest<string>();

            return Deleted<string>("");
        }

        #endregion
    }
}
