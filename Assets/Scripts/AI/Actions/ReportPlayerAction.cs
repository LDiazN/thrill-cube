using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ReportPlayer", story: "Report [player] to my area (using my [perceptions] )", category: "Action", id: "900e30136f2518ebd0320c8df476151c")]
public partial class ReportPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<Player> Player;
    [SerializeReference] public BlackboardVariable<Perceptions> Perceptions;

    protected override Status OnStart()
    {
        var perceptions = Perceptions.Value;
        perceptions.PlayerDiscovered(Player.Value);
        return Status.Success;
    }
}

