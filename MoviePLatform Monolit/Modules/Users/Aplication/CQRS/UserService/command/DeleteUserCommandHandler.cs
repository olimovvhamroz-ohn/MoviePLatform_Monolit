namespace MoviePLatform_Monolit.Modules.Users.Aplication.CQRS.UserService.command;
using MediatR;

public record DeleteUserCommand(long Id) : IRequest<Unit>;
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Unit> Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        await _userRepository.DeleteAsync(
            request.Id,
            cancellationToken);

        return Unit.Value;
    }
}