using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckAlive", story: "Check [Health] to see if [alive]", category: "Action", id: "d489d95f3dd5ce0c91c69bef14ffc187")]
public partial class CheckAliveAction : Action
{
    [SerializeReference] public BlackboardVariable<Health> Health;
    [SerializeReference] public BlackboardVariable<bool> Alive;
    protected override Status OnStart()
    {
        var health = Health.Value;
        Alive.Value = !health.isDead;
        return health.isDead ? Status.Failure : Status.Success;
    }
}

