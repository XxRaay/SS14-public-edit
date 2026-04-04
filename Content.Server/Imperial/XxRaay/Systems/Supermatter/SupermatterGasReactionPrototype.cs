using Content.Shared.Atmos;
using Robust.Shared.Prototypes;

namespace Content.Server.Imperial.XxRaay.Systems.Supermatter;

[Prototype("supermatterGasReaction"), DataDefinition]
public sealed partial class SupermatterGasReactionPrototype : IPrototype
{
    [ViewVariables]
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField(required: true)]
    public Gas Gas { get; private set; }

    [DataField(required: true)]
    public ISupermatterGasReaction Reaction { get; private set; } = default!;
}
