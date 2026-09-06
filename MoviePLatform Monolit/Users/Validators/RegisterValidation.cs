using FluentValidation;
using MoviePLatform_Monolit.Users.DTO.REQUEST;

public class RegisterValidation : AbstractValidator<RegisterUserRequest>
{
    public RegisterValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Имя обязательно")
            .MinimumLength(2).WithMessage("Минимум 2 символа")
            .MaximumLength(50).WithMessage("Максимум 50 символов");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен")
            .EmailAddress().WithMessage("Некорректный Email");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен")
            .MinimumLength(6).WithMessage("Минимум 6 символов");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow).WithMessage("Дата рождения не может быть в будущем")
            .When(x => x.DateOfBirth.HasValue);
    }
}