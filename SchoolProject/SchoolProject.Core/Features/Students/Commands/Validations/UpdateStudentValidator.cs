// Ignore Spelling: Validator Vadlidatoins

using FluentValidation;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Features.Students.Commands.Models;
using SchoolProject.Core.Resources;
using SchoolProject.Service.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProject.Core.Features.Students.Commands.Vadlidatoins
{
    public class UpdateStudentValidator : AbstractValidator<UpdateStudentCommand>
    {
        #region Fields
        private readonly IStudentService _studentService;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;

        #endregion

        #region Constructors
        public UpdateStudentValidator(IStudentService studentService , 
            IStringLocalizer<SharedResources> stringLocalizer)
        {
            _studentService = studentService;
            _stringLocalizer = stringLocalizer;
            ApplyValidationRules();
            ApplyCustomValidationRules();
        }
        #endregion

        #region Actions
        public void ApplyValidationRules()
        {
            RuleFor(s => s.Name)
                .NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
                .NotNull().WithMessage(_stringLocalizer[SharedResourcesKeys.Required])
                .MaximumLength(50).WithMessage(_stringLocalizer[SharedResourcesKeys.MaxLengthIs100]);


            RuleFor(s => s.Address)
                .NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
                .NotNull().WithMessage(_stringLocalizer[SharedResourcesKeys.Required])
                .MaximumLength(50).WithMessage(_stringLocalizer[SharedResourcesKeys.MaxLengthIs100]);

        }


        public async void ApplyCustomValidationRules()
        {
            RuleFor(s => s.Name)
                .MustAsync(async (Model , Key, CancellationToken) =>
                // Why "!" because if it name was found then it's true
                           !await _studentService.IsNameExistsExcludeSelf(Key , Model.StudID))
                .WithMessage("This Name is already Exists");
        } 
        #endregion

    }
}
