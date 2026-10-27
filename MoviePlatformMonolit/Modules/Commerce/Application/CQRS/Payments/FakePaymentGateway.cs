using MoviePlatformMonolit.Modules.Command.Interfaces;

namespace MoviePlatformMonolit.Modules.Commerce.Application.CQRS.Payments;

public class FakePaymentGateway:IPaymentGateway
{
    public Task<bool> PayAsync(long userId, decimal amount)
    {
        return Task.FromResult(true);
    }
}