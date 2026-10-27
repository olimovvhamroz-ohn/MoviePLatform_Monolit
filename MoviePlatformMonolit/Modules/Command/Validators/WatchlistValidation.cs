
using FluentValidation;
using MoviePlatformMonolit.Modules.Engagement.Domain.Dto.REQUEST;

namespace MoviePlatformMonolit.Modules.Command.Validators;

public class WatchlistValidation : AbstractValidator<WatchlistRequest>
{
    public WatchlistValidation()
    {

        RuleFor(x => x.MovieId)
            .GreaterThan(0)
            .WithMessage("Некорректный ID фильма");
    }
}