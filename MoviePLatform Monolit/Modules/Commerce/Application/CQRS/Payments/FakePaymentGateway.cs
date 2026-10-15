using MoviePLatform_Monolit.Modules.Command.Interfaces;

namespace MoviePLatform_Monolit.Modules.Commerce.Application.CQRS.Payments;

public class FakePaymentGateway:IPaymentGateway
{
    public Task<bool> PayAsync(long userId, decimal amount)
    {
        return Task.FromResult(true);
    }
}