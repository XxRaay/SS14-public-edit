using Content.Shared.Examine;
using Robust.Server.Player;

namespace Content.Server.Imperial.Subscriptions;

public sealed class SubscriptionExamineSystem : EntitySystem
{
    [Dependency] private readonly SubscriptionManager _subscriptions = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<ActorComponent, ExaminedEvent>(OnExamined);
    }

    private void OnExamined(Entity<ActorComponent> ent, ref ExaminedEvent args)
    {
        if (!_subscriptions.TryGetTier(ent.Comp.PlayerSession.UserId, out var tier))
            return;

        args.PushMarkup($"[color={tier.Value.OocColor()}]Subscription: {tier.Value.DisplayName()}[/color]");
    }
}
