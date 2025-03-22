using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CloseDistance", story: "I'll close my [range] to [target]", category: "Action", id: "f3e5f53b5beb9f2ad3a0908b7e0cceaa")]
public partial class CloseDistanceAction : Action
{
    [SerializeReference] public BlackboardVariable<RangeDetector> Range;
    [SerializeReference] public BlackboardVariable<Player> Target;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var player = Target.Value;
        if (player == null)
            return Status.Failure;
        
        var navMeshAgent = GameObject.GetComponent<NavMeshAgent>();
        var rangeDetector = Range.Value;

        if (rangeDetector.IsPlayerInRange())
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.SetDestination(navMeshAgent.transform.position);
            return Status.Success;
        }
        
        navMeshAgent.SetDestination(player.transform.position);
        navMeshAgent.isStopped = false;
        return Status.Failure;
    }
}

