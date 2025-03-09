using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Check player in range", story: "I'll check my [rangeDetector] to find [Player]", category: "Action", id: "0cba7c82306e20fa8ff13e9ef4e6f6aa")]
public partial class CheckPlayerInRangeAction : Action
{
    [SerializeReference] public BlackboardVariable<RangeDetector> RangeDetector;
    [SerializeReference] public BlackboardVariable<Player> Player;

    protected override Status OnStart()
    {
        var rangeDetector = RangeDetector.Value;
        if (rangeDetector.IsPlayerInRange())
        {
            Player.Value = rangeDetector.Player;
            return Status.Success;
        }
        return Status.Failure;
    }

}

