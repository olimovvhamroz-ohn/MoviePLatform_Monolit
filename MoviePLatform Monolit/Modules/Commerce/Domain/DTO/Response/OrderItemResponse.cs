namespace MoviePLatform_Monolit.Modules.Commerce.Domain.DTO.Response;

public class OrderItemResponse
{
    public long MovieId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}