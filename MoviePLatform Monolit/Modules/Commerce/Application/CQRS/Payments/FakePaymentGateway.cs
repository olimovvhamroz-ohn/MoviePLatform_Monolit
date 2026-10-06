using MoviePLatform_Monolit.Modules.SharedKernel.Interfaces;

namespace MoviePLatform_Monolit.Modules.Commerce.Application.CQRS.Payments;


public class FakePaymentGateway:IPaymentGateway
{
    public Task<bool> PayAsync(int userId, decimal amount)
    {
        return Task.FromResult(true);
    }
}