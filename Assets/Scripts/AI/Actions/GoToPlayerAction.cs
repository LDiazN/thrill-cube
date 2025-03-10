using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GoToPlayer", story: "I'll head to the [player]", category: "Action", id: "05fb76fab7e3f84eaed9cc38a562ac0b")]
public partial class GoToPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<Player> Player;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var navMeshAgent = GameObject.GetComponent<NavMeshAgent>();
        var player = Player.Value;
        
        if (player == null)
            return Status.Failure;
        
        if (!navMeshAgent.isActiveAndEnabled)
            return Status.Failure;
        
        navMeshAgent.SetDestination(player.transform.position);
        
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            return Status.Success;
        
        return Status.Running;
    }
}

