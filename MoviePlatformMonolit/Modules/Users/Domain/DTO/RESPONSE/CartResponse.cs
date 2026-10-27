using MoviePlatformMonolit.Modules.Commerce.Domain.DTO.Response;

namespace MoviePlatformMonolit.Modules.Users.Domain.DTO.RESPONSE;

public class CartResponse
{
    public long UserId { get; set; }
    public bool IsCheckedOut { get; set; }
    public ICollection<CartItemResponse> CartItems { get; set; }
        = new List<CartItemResponse>();
}