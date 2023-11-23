using FluentValidation;
using ResourceStorageService.Application.Commands;

namespace ResourceStorageService.Application.Validations
{
    public class UploadMultipleResourcesCommandValidator : AbstractValidator<UploadMultipleResourcesCommand>
    {
        public UploadMultipleResourcesCommandValidator()
        {
            ValidatorOptions.Global.CascadeMode = CascadeMode.Stop;
            RuleFor(document => document.Resources).NotEmpty().WithMessage("Resources must not be null or empty");
        }
    }
}
