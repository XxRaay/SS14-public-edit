using System;
using Content.Server.Actions;
using Content.Server.Popups;
using Content.Shared.Actions;
using Content.Shared.Imperial.Seriozha.SCP.Events;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Server.Imperial.Seriozha.SCP.Systems;

/// <summary>
/// Handles the SCP mask targeted ability: equips a specific mask into the target's head slot and paralyzes them.
/// </summary>
public sealed class SCPMaskActionSystem : EntitySystem
{
    [Dependency] private readonly SharedStunSystem _stun = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly ActionsSystem _actions = default!;

    private const string HeadSlot = "head"; // standard head/helmet slot name
    private const string MaskPrototype = "ImperialSCPMask"; // existing prototype id in SCP.yml

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SCPMaskActionEvent>(OnPerform);
    }

    private void OnPerform(SCPMaskActionEvent ev)
    {
        if (ev.Handled)
            return;

        var target = ev.Target;
        if (Deleted(target))
            return;

        // range check is handled by TargetActionComponent, but ensure target has inventory
        if (!TryComp<InventoryComponent>(target, out var inv))
        {
            if (ev.Performer != EntityUid.Invalid)
                _popup.PopupEntity(Loc.GetString("comp-clothing-unequip-cannot"), ev.Performer, ev.Performer, PopupType.Medium);
            return;
        }

        // If head slot occupied, try to unequip forcefully
        if (_inventory.TryGetSlotEntity(target, HeadSlot, out var curHead, inv))
        {
            // Ignore if already our mask
            if (curHead is { } head && TryComp(head, out MetaDataComponent? meta) && meta.EntityPrototype?.ID == MaskPrototype)
            {
                // Already masked – still apply stun
                var stunTime = ev.StunSeconds > 0 ? ev.StunSeconds : 7f;
                _stun.TryParalyze(target, TimeSpan.FromSeconds(stunTime), true);
                ev.Handled = true;
                return;
            }

            _inventory.TryUnequip(target, HeadSlot, silent: true, force: true, inventory: inv);
        }

        // Spawn mask and equip
        var mask = Spawn(MaskPrototype, Transform(target).Coordinates);

        // Add unremovable behavior so the victim cannot remove it themself. Use SelfUnremovableClothing so others can strip.
        EnsureComp<Content.Shared.Clothing.Components.SelfUnremovableClothingComponent>(mask);

        // Equip forcibly into head slot
        _inventory.TryEquip(target, mask, HeadSlot, silent: true, force: true, inventory: inv);

        // Paralyze target
        var finalStun = ev.StunSeconds > 0 ? ev.StunSeconds : 7f;
        _stun.TryParalyze(target, TimeSpan.FromSeconds(finalStun), true);

        ev.Handled = true;
    }
}

