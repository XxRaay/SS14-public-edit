using Content.Server.Imperial.Power.Components;

namespace Content.Server.Imperial.Power.EntitySystems.Events;

/// <summary>
/// Событие "Ничего" - период спокойствия суперматерии
/// </summary>
[DataDefinition]
public sealed class SupermatterNoneEvent : ISupermatterEvent
{
    public SupermatterEventComponent.SupermatterEventType Type => SupermatterEventComponent.SupermatterEventType.None;

    public void Activate(Entity<SupermatterEventComponent> entity, SupermatterEventSystem supermatterSystem)
    {
        if (entity.AsType() == EntityUid.Invalid)
        {
            supermatterSystem.Log.Error("SupermatterNoneEvent.Activate: Invalid EntityUid provided");
            return;
        }

        var currentTime = supermatterSystem.GameTiming.CurTime;
        entity.Comp.CurrentEvent = SupermatterEventComponent.SupermatterEventType.None;
        entity.Comp.EventEndTime = TimeSpan.Zero;
        entity.Comp.NextEventTimer = entity.Comp.NoneEventDuration;
        entity.Comp.LastEventEndTimeUpdate = currentTime;
        entity.Comp.LastNextEventTimerUpdate = currentTime;
    }

    public void Process(Entity<SupermatterEventComponent> _, SupermatterEventSystem __, TimeSpan ___)
    {
    }

    public string GetAnnouncement()
    {
        return Loc.GetString("supermatter-event-none");
    }
}
