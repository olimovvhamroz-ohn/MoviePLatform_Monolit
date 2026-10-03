
namespace MoviePLatform_Monolit.Modules.Users.Domain.DTO.RESPONSE;

public class EntitlementResponse
{
    public bool CanWatch { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime? RentalStartDeadline { get; set; }
    public DateTime? FirstWatchedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}