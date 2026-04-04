using Content.Server.Imperial.Power.Components;
using Content.Shared.Damage.Components;

namespace Content.Server.Imperial.Power.EntitySystems.Events;

/// <summary>
/// Событие "Молния" - суперматерия генерирует электрические разряды
/// </summary>
[DataDefinition]
public sealed class SupermatterLightningEvent : ISupermatterEvent
{
    public SupermatterEventComponent.SupermatterEventType Type => SupermatterEventComponent.SupermatterEventType.Lightning;

    public void Activate(Entity<SupermatterEventComponent> entity, SupermatterEventSystem supermatterSystem)
    {
        if (entity.AsType() == EntityUid.Invalid)
        {
            supermatterSystem.Log.Error("SupermatterLightningEvent.Activate: Invalid EntityUid provided");
            return;
        }

        var currentTime = supermatterSystem.GameTiming.CurTime;
        entity.Comp.CurrentEvent = SupermatterEventComponent.SupermatterEventType.Lightning;
        entity.Comp.EventEndTime = entity.Comp.LightningEventDuration;
        entity.Comp.NextEventTimer = entity.Comp.EventAfterLightingTime;
        entity.Comp.LightningCooldown = TimeSpan.Zero;
        entity.Comp.LastEventEndTimeUpdate = currentTime;
        entity.Comp.LastNextEventTimerUpdate = currentTime;
        entity.Comp.LastLightningCooldownUpdate = currentTime;

        // Стреляем молнии в случайные цели вокруг суперматерии
        ShootRandomLightnings(entity, supermatterSystem);
    }

    public void Process(Entity<SupermatterEventComponent> entity, SupermatterEventSystem supermatterSystem, TimeSpan currentTime)
    {
        var elapsedSinceLastUpdate = currentTime - entity.Comp.LastLightningCooldownUpdate;
        entity.Comp.LightningCooldown -= elapsedSinceLastUpdate;
        entity.Comp.LastLightningCooldownUpdate = currentTime;

        if (entity.Comp.LightningCooldown > TimeSpan.Zero)
            return;

        // Стреляем молнии в случайные цели вокруг суперматерии
        ShootRandomLightnings(entity, supermatterSystem);

        if (supermatterSystem.TryGetComponent<SupermatterIntegrityComponent>(entity, out var integrity) && integrity != null &&
            supermatterSystem.TryGetComponent<DamageableComponent>(entity, out _))
        {
            supermatterSystem.Damageable.TryChangeDamage(entity.Owner, integrity.TickDamage, origin: null);
        }

        entity.Comp.LightningCooldown = entity.Comp.LightningCooldownDuration;
    }

    private void ShootRandomLightnings(Entity<SupermatterEventComponent> entity, SupermatterEventSystem supermatterSystem)
    {
        // Используем ShootRandomLightnings для стрельбы в случайные цели в радиусе
        supermatterSystem.LightningSystem.ShootRandomLightnings(entity, entity.Comp.LightningBoltRadius, entity.Comp.LightningBoltCount);
    }

    public string GetAnnouncement()
    {
        return Loc.GetString("supermatter-event-lightning");
    }
}
