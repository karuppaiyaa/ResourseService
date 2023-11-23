using FluentValidation;

namespace ResourceStorageService.Application.Validations
{
    public class CustomValidator<T> : AbstractValidator<T>
    {
        public CustomValidator()
        {
            ValidatorOptions.Global.CascadeMode = CascadeMode.Stop;

            RuleFor(x => x).NotEmpty().WithMessage("Input value cannot be null or empty");
        }
    }
}
