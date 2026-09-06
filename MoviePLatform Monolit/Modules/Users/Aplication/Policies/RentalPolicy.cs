

public static class RentalPolicy
{
    public static readonly TimeSpan StartWindow = TimeSpan.FromDays(30);
    public static readonly TimeSpan PlaybackWindow = TimeSpan.FromHours(48);

    public static DateTime GetStartDeadline(DateTime purchasedAt) => purchasedAt.Add(StartWindow);

    public static DateTime? GetExpiresAt(DateTime? firstWatchedAt) =>
        firstWatchedAt.HasValue ? firstWatchedAt.Value.Add(PlaybackWindow) : null;
}