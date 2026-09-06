using FluentValidation;
using MoviePLatform_Monolit.Users.DTO.REQUEST;

public class PurchaseValidation : AbstractValidator<PurchaseRequest>
{
    public PurchaseValidation()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("Некорректный ID пользователя");

        RuleFor(x => x.MovieId)
            .GreaterThan(0)
            .WithMessage("Некорректный ID фильма");
    }
}