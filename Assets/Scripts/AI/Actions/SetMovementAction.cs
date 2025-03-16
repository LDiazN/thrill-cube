using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetMovement", story: "Set this navmesh agent movement to: [value]", category: "Action", id: "b3ac1d5707522ac2516c422e7ee4a1a1")]
public partial class SetMovementAction : Action
{
    [SerializeReference] public BlackboardVariable<bool> Value;

    protected override Status OnStart()
    {
        var navMeshAgent = GameObject.GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
            return Status.Success;

        var shouldMove = Value.Value;
        navMeshAgent.isStopped = !shouldMove;
        return Status.Success;
    }
}

