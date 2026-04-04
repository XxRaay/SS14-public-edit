namespace Content.Server.Imperial.Subscriptions;

public enum SubscriptionTier
{
    None = 0,
    Explorer = 1,
    Vanguard = 2,
    Sovereign = 3,
}

public static class SubscriptionTierExt
{
    public static string DisplayName(this SubscriptionTier tier) => tier switch
    {
        SubscriptionTier.Explorer => "Explorer",
        SubscriptionTier.Vanguard => "Vanguard",
        SubscriptionTier.Sovereign => "Sovereign",
        _ => "None",
    };

    public static string OocColor(this SubscriptionTier tier) => tier switch
    {
        SubscriptionTier.Explorer => "#4FC3F7",
        SubscriptionTier.Vanguard => "#BA68C8",
        SubscriptionTier.Sovereign => "#FFD54F",
        _ => "#FFFFFF",
    };

    public static int AntagWeight(this SubscriptionTier tier) => tier switch
    {
        SubscriptionTier.Explorer => 125,
        SubscriptionTier.Vanguard => 150,
        SubscriptionTier.Sovereign => 200,
        _ => 100,
    };
}
