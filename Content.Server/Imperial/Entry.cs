using Content.Server.Imperial.Sponsors;
using Content.Server.Imperial.Subscriptions;

namespace Content.Server.Imperial.Entry;


public sealed partial class ImperialEntry
{
    public static void Init()
    {
        IoCManager.Resolve<SponsorsManager>().Initialize();
        IoCManager.Resolve<SubscriptionManager>();
    }

    public static void PostInit()
    {

    }

    public static void IoCRegister(IDependencyCollection deps)
    {
        deps.Register<SponsorsManager>();
        deps.Register<SubscriptionManager>();
    }
}
