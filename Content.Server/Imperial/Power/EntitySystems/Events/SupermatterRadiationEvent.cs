using Content.Server.Imperial.Power.Components;

namespace Content.Server.Imperial.Power.EntitySystems.Events;

/// <summary>
/// Событие "Радиация" - суперматерия излучает повышенную радиацию
/// </summary>
[DataDefinition]
public sealed class SupermatterRadiationEvent : ISupermatterEvent
{
    public SupermatterEventComponent.SupermatterEventType Type => SupermatterEventComponent.SupermatterEventType.Radiation;

    public void Activate(Entity<SupermatterEventComponent> entity, SupermatterEventSystem supermatterSystem)
    {
        if (entity.AsType() == EntityUid.Invalid)
        {
            supermatterSystem.Log.Error("SupermatterRadiationEvent.Activate: Invalid EntityUid provided");
            return;
        }

        var currentTime = supermatterSystem.GameTiming.CurTime;
        entity.Comp.CurrentEvent = SupermatterEventComponent.SupermatterEventType.Radiation;
        entity.Comp.EventEndTime = entity.Comp.RadiationEventDuration;
        entity.Comp.NextEventTimer = entity.Comp.EventAfterRadiationTime;
        entity.Comp.LastEventEndTimeUpdate = currentTime;
        entity.Comp.LastNextEventTimerUpdate = currentTime;

        supermatterSystem.SetRadiation(entity, entity.Comp.RadiationEventIntensity);
    }

    public void Process(Entity<SupermatterEventComponent> entity, SupermatterEventSystem supermatterSystem, TimeSpan currentTime)
    {
        supermatterSystem.SetRadiation(entity, entity.Comp.RadiationEventIntensity);
    }

    public string GetAnnouncement()
    {
        return Loc.GetString("supermatter-event-radiation");
    }
}
