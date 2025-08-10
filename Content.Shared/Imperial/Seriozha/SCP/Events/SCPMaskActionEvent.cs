using Content.Shared.Actions;

namespace Content.Shared.Imperial.Seriozha.SCP.Events;

/// <summary>
/// Entity-target action event for the SCP mask ability.
/// Select a target; on server this will equip a mask to the target's head and paralyze them.
/// </summary>
public sealed partial class SCPMaskActionEvent : EntityTargetActionEvent
{
    /// <summary>
    /// Paralyze duration in seconds for the target.
    /// </summary>
    [DataField]
    public float StunSeconds = 7f;
}


