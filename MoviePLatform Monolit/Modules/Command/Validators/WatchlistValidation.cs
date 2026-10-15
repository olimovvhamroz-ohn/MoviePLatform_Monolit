
using FluentValidation;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Dto.REQUEST;

namespace MoviePLatform_Monolit.Modules.Command.Validators;

public class WatchlistValidation : AbstractValidator<WatchlistRequest>
{
    public WatchlistValidation()
    {

        RuleFor(x => x.MovieId)
            .GreaterThan(0)
            .WithMessage("Некорректный ID фильма");
    }
}