using FluentValidation;
using MoviePLatform_Monolit.Users.DTO.REQUEST;


public class UserValidation : AbstractValidator<UserRequest>
{
    public UserValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Имя обязательно")
            .MinimumLength(2).WithMessage("Минимум 2 символа")
            .MaximumLength(50).WithMessage("Максимум 50 символов");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Телефон обязателен");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен")
            .EmailAddress().WithMessage("Некорректный Email");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен")
            .MinimumLength(6).WithMessage("Минимум 6 символов");
    }
}