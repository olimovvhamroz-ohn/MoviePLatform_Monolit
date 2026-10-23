namespace MoviePLatform_Monolit.Modules.Command.Interfaces;

public interface IPaymentGateway
{
    Task<bool> PayAsync(long userId, decimal amount);
}