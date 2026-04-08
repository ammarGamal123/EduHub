using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Resources;
using System.Text;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators,
                              IStringLocalizer<SharedResources> stringLocalizer = null)
    {
        _validators = validators;
        _stringLocalizer = stringLocalizer;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Count != 0)
            {
                var errorMessages = new StringBuilder();

                foreach (var failure in failures)
                {
                    var localizedPropertyName = _stringLocalizer != null
                        ? _stringLocalizer[failure.PropertyName]
                        : failure.PropertyName;

                    errorMessages.AppendLine($"{localizedPropertyName}: {failure.ErrorMessage}");
                }

                throw new ValidationException(errorMessages.ToString());
            }
        }
        return await next();
    }
}
