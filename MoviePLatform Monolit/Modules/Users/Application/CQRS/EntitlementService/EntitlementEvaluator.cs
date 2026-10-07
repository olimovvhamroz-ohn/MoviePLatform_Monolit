using MoviePLatform_Monolit.Entity;
using MoviePLatform_Monolit.Modules.Users.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Users.Domain.Enums;


public static class EntitlementEvaluator
{
    public static MovieAccessResultResponse CheckAccess(
        UserEntity user,
        MovieEntity movie,
        PurchaseEntity? purchase,
        DateTime nowUtc)
    {
        var minAge = (int)movie.AgeRating;
        if (minAge > 0)
        {
            var userAge = CalculateAge(user.DateOfBirth, nowUtc);

            if (userAge == null || userAge < minAge)
            {
                return new MovieAccessResultResponse
                {
                    CanWatch = false,
                    Reason = EntitlementReason.AgeRestricted.ToString(),
                    Message = $"Age restriction: {minAge}+"
                };
            }
        }

        if (purchase == null)
        {
            return new MovieAccessResultResponse
            {
                CanWatch = false,
                Reason = EntitlementReason.NotPurchased.ToString(),
                Message = "The movie has not been purchased."
            };
        }

        var startDeadline = RentalPolicy.GetStartDeadline(purchase.PurchasedAt);
        var expiresAt = RentalPolicy.GetExpiresAt(purchase.FirstWatchedAt);

        if (purchase.FirstWatchedAt == null && nowUtc > startDeadline)
        {
            return new MovieAccessResultResponse
            {
                CanWatch = false,
                Reason = EntitlementReason.RentalStartWindowExpired.ToString(),
                Message = "The 30-day window to start watching has expired.",
                RentalStartDeadline = startDeadline
            };
        }

        if (purchase.FirstWatchedAt != null && nowUtc > expiresAt)
        {
            return new MovieAccessResultResponse
            {
                CanWatch = false,
                Reason = EntitlementReason.RentalPlaybackWindowExpired.ToString(),
                Message = "The 48-hour viewing window has expired.",
                FirstWatchedAt = purchase.FirstWatchedAt,
                ExpiresAt = expiresAt
            };
        }

        return new MovieAccessResultResponse
        {
            CanWatch = true,
            Reason = EntitlementReason.Allowed.ToString(),
            Message = "Access granted.",
            RentalStartDeadline = startDeadline,
            FirstWatchedAt = purchase.FirstWatchedAt,
            ExpiresAt = expiresAt
        };
    }

    private static int? CalculateAge(DateTime? dateOfBirth, DateTime nowUtc)
    {
        if (dateOfBirth == null)
            return null;

        var birthDate = dateOfBirth.Value;
        var age = nowUtc.Year - birthDate.Year;

        if (birthDate.Date > nowUtc.AddYears(-age).Date)
            age--;

        return age;
    }
}