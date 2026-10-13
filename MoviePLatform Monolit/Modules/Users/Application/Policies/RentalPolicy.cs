public static class RentalPolicy
{
    public static readonly TimeSpan StartWindow = TimeSpan.FromDays(30);

    public static readonly TimeSpan PlaybackWindow = TimeSpan.FromHours(48);

    public static DateTime? GetExpiresAt(DateTime? firstWatchedAt)
    {
        if (firstWatchedAt == null)
        {
            return null;
        }

        var res = firstWatchedAt.Value.Add(PlaybackWindow);
        return res;
    }

    public static DateTime GetStartDeadline(DateTime purchaseAt)
    {
        var res = purchaseAt.Add(StartWindow);
        return res;
    }
}