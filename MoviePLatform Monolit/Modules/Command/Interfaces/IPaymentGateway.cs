namespace MoviePLatform_Monolit.Modules.SharedKernel.Interfaces;

public interface IPaymentGateway
{
   
        Task<bool> PayAsync(long userId, decimal amount);
} 
