namespace MoviePLatform_Monolit.Modules.Command.Extensions;

public class NotFoundException(string message) : Exception(message);

public class BadRequestException(string message) : Exception(message);

public class UnauthorizedException(string message) : Exception(message);