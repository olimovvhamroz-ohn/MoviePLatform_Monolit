
using MoviePLatform_Monolit.Entity;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

public static class EntitlementEvaluator
{
    public static EntitlementResponse Evaluate(UserEntity user, MovieEntity movie, PurchaseEntity? purchase, DateTime nowUtc)
    {
        if ((int)movie.AgeRating > 0)
        {
            var age = CalculateAge(user.DateOfBirth, nowUtc);
            if (age is null || age <(int) movie.AgeRating)
            {
                return Deny(
                    EntitlementReason.AgeRestricted,
                    $"Фильм имеет возрастное ограничение {movie.AgeRating}+, доступ запрещён.");
            }
        }

        if (purchase == null)
        {
            return Deny(EntitlementReason.NotPurchased, "Фильм не куплен и недоступен для просмотра.");
        }

        var startDeadline = RentalPolicy.GetStartDeadline(purchase.PurchasedAt);

        if (purchase.FirstWatchedAt == null)
        {
            if (nowUtc > startDeadline)
            {
                var expired = Deny(
                    EntitlementReason.RentalStartWindowExpired,
                    "Истекли 30 дней, отведённые на начало просмотра. Аренда закрыта.");
                expired.RentalStartDeadline = startDeadline;
                return expired;
            }

            var allowedNotStarted = Allow("Доступ разрешён. Отсчёт 48 часов начнётся при первом запуске просмотра.");
            allowedNotStarted.RentalStartDeadline = startDeadline;
            return allowedNotStarted;
        }

        var expiresAt = RentalPolicy.GetExpiresAt(purchase.FirstWatchedAt);

        if (nowUtc > expiresAt)
        {
            var expiredPlayback = Deny(
                EntitlementReason.RentalPlaybackWindowExpired,
                "Истекли 48 часов доступа после первого просмотра. Аренда закрыта.");
            expiredPlayback.FirstWatchedAt = purchase.FirstWatchedAt;
            expiredPlayback.ExpiresAt = expiresAt;
            return expiredPlayback;
        }

        var allowed = Allow("Доступ разрешён.");
        allowed.FirstWatchedAt = purchase.FirstWatchedAt;
        allowed.ExpiresAt = expiresAt;
        return allowed;
    }

    private static EntitlementResponse Allow(string message) => new()
    {
        CanWatch = true,
        Reason = nameof(EntitlementReason.Allowed),
        Message = message
    };

    private static EntitlementResponse Deny(EntitlementReason reason, string message) => new()
    {
        CanWatch = false,
        Reason = reason.ToString(),
        Message = message
    };

    private static int? CalculateAge(DateTime? dateOfBirth, DateTime nowUtc)
    {
        if (dateOfBirth == null) return null;

        var dob = dateOfBirth.Value;
        var age = nowUtc.Year - dob.Year;
        if (dob.Date > nowUtc.AddYears(-age).Date) age--;
        return age;
    }
}