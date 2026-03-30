// Ignore Spelling: Validator

using FluentValidation;
using SchoolProject.Core.Features.Students.Commands.Models;
using SchoolProject.Service.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProject.Core.Features.Students.Commands.Validations
{
    public class DeleteStudentValidator : AbstractValidator<DeleteStudentCommand>
    {
        private readonly IStudentService _studentService;

        public DeleteStudentValidator(IStudentService studentService)
        {
            _studentService = studentService;
        }


        public DeleteStudentValidator()
        {
            ApplyCustomValidation();
        }

        public async void ApplyCustomValidation()
        {
            RuleFor(s => s.StudID)
                .MustAsync(async (Model, Key, CancellationToken) =>
                            await _studentService.IsStudentIdExist(Model.StudID))
                .WithMessage($"No Student Found With This ID");
        }
    }
}
