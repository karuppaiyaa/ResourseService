using FluentValidation;
using ResourceStorageService.Application.Commands;

namespace ResourceStorageService.Application.Validations
{
    public class UploadResourceValidator : AbstractValidator<UploadResourceCommand>
    {
        public UploadResourceValidator()
        {
            RuleFor(x => x.Resource).NotEmpty().WithMessage("Resource must not be null or empty");
        }
    }
}
