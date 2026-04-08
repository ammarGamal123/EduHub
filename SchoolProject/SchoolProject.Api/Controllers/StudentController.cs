using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolProject.Api.Base;
using SchoolProject.Core.Features.Students.Commands.Models;
using SchoolProject.Core.Features.Students.Queries.Models;
using SchoolProject.Data.AppMetaData;

namespace SchoolProject.Api.Controllers
{
    [ApiController]
    public class StudentController : AppControllerBase
    {
        public StudentController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet(Router.StudentRouting.List)]
        public async Task<IActionResult> GetAllStudents()
        {
            var response = await _mediator.Send(new GetStudentsListQuery());

            return Ok(response);
        }

        [HttpGet(Router.StudentRouting.GetByID)]
        public async Task<IActionResult> GetStudentByIDAsync([FromRoute] int id)
        {
            var response = await _mediator.Send(new GetStudentByIDQuery(id));

            return NewResult(response);
        }

        [HttpGet(Router.StudentRouting.Paginated)]
        public async Task<IActionResult> GetStudentsPaginatedList
            ([FromQuery] GetStudentPaginatedListQuery query)
        {
            var response = await _mediator.Send(query);

            return Ok(response);
        } 


        [HttpPost(Router.StudentRouting.Create)]
        public async Task<IActionResult> CreateStudentAsync
            ([FromBody] CreateStudentCommand command)
        {
            var response = await _mediator.Send(command);

            return NewResult(response);
        }

        [HttpPut(Router.StudentRouting.Update)]
        public async Task<IActionResult> UpdateStudentAsync([FromBody] UpdateStudentCommand command)
        {
            var response = await _mediator.Send(command);

            return NewResult(response);
        }

        [HttpDelete(Router.StudentRouting.Delete)]
        public async Task<IActionResult> DeleteStudentAsync([FromRoute] int StudID)
        {
            var resposne = await _mediator.Send(new DeleteStudentCommand(StudID));

            return NewResult(resposne);
        }
    }
}