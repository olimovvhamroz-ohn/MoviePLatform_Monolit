
using FluentValidation;

public class WatchlistValidation : AbstractValidator<WatchlistRequest>
{
    public WatchlistValidation()
    {
        /*RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("Некорректный ID пользователя");*/

        RuleFor(x => x.MovieId)
            .GreaterThan(0)
            .WithMessage("Некорректный ID фильма");
    }
}