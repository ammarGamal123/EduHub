using FluentValidation;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Features.Students.Commands.Models;
using SchoolProject.Core.Resources;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Core.Features.Students.Commands.Validations
{
    public class CreateStudentValidator : AbstractValidator<CreateStudentCommand>
    {
        #region Fields
        private readonly IStudentService _studentService;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public CreateStudentValidator(IStudentService studentService, IStringLocalizer<SharedResources> stringLocalizer)
        {
            _studentService = studentService;
            _stringLocalizer = stringLocalizer;

            AddBasicValidationRules();
            AddCustomValidationRules();
        }
        #endregion

        #region Actions
        private void AddBasicValidationRules()
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

        private void AddCustomValidationRules()
        {
            RuleFor(s => s.Name)
                .MustAsync(async (name, cancellationToken) =>
                    !await _studentService.IsNameExists(name.ToLower()))
                // Ensuring case-insensitive comparison
                .WithMessage(_stringLocalizer[SharedResourcesKeys.IsExist]);
        }
        #endregion
    }
}
