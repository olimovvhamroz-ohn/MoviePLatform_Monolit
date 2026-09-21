namespace MoviePLatform_Monolit.Modules.Commerce.Domain.DTO.RESPONSE;

public class PaymentResponse
{
    public long Id { get; set; }
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class PaymentProcessResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
    public PaymentResponse? PaymentDetails { get; set; }
}