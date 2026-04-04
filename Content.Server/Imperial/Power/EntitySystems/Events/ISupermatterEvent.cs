using Content.Server.Imperial.Power.Components;

namespace Content.Server.Imperial.Power.EntitySystems.Events;

[ImplicitDataDefinitionForInheritors]
public interface ISupermatterEvent
{
    SupermatterEventComponent.SupermatterEventType Type { get; }

    void Activate(Entity<SupermatterEventComponent> entity, SupermatterEventSystem supermatterSystem);

    void Process(Entity<SupermatterEventComponent> entity, SupermatterEventSystem supermatterSystem, TimeSpan currentTime);

    string GetAnnouncement();
}
