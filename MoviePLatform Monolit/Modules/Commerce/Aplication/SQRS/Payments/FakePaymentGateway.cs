namespace MoviePLatform_Monolit.Modules.Commerce.Aplication.SQRS.Payments;

public interface IPaymentGeteway
{
    Task<bool> PayAsync(int userId, decimal amount);
}
public class FakePaymentGateway:IPaymentGeteway
{
    public Task<bool> PayAsync(int userId, decimal amount)
    {
        return Task.FromResult(true);
    }
}