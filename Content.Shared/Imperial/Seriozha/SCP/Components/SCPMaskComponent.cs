using Robust.Shared.GameStates;

namespace Content.Shared.Imperial.Seriozha.SCP.Components;

/// <summary>
/// Marker component that grants the SCP mask action on spawn and stores config.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class SCPMaskComponent : Component
{
    /// <summary>
    /// Prototype id of the action entity to grant.
    /// </summary>
    [DataField]
    public string ActionPrototype = "ActionSCPMask";

    /// <summary>
    /// The actual action entity reference when granted.
    /// </summary>
    [ViewVariables]
    public EntityUid? Action;

    /// <summary>
    /// Default stun time if event does not override.
    /// </summary>
    [DataField]
    public float StunSeconds = 7f;
}


