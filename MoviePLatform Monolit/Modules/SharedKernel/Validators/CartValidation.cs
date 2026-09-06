using FluentValidation;
using MoviePLatform_Monolit.Users.DTO.REQUEST;

public class CartValidation : AbstractValidator<CartRequest>
{
    public CartValidation()
    {

        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("Некорректный ID пользователя");
    }
}