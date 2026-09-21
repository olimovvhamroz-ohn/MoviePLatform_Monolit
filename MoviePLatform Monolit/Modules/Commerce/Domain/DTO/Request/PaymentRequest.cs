namespace MoviePLatform_Monolit.Modules.Commerce.Domain.DTO.REQUEST;

public class ProcessPaymentRequest
{
    public int OrderId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentDetails? CardDetails { get; set; }
}

public class PaymentDetails
{
    public string CardNumber { get; set; } = string.Empty;
    public string CardHolder { get; set; } = string.Empty;
    public string ExpiryDate { get; set; } = string.Empty;
    public string CVV { get; set; } = string.Empty;
}

public class RefundPaymentRequest
{
    public int PaymentId { get; set; }
    public string? Reason { get; set; }
}

public class VerifyPaymentRequest
{
    public int PaymentId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
}