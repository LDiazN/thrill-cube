using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GoToEnemy", story: "Go to [enemy] until my distance is less than [distance] and my [range] says I have line of sight", category: "Action", id: "00dfa35d62fb4f274f6f1f6d9388b601")]
public partial class GoToEnemyAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Enemy;
    [SerializeReference] public BlackboardVariable<float> Distance;
    [SerializeReference] public BlackboardVariable<RangeDetector> Range;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var navMeshAgent = GameObject.GetComponent<NavMeshAgent>();
        var enemy = Enemy.Value;
        var range = Range.Value;
        if (enemy == null)
            return Status.Failure;
        
        if (!navMeshAgent.isActiveAndEnabled)
            return Status.Failure;
        
        navMeshAgent.SetDestination(enemy.transform.position);

        LayerMask ignore = (1 << LayerMask.NameToLayer("Player"));
        if (range.HasLineOfSight(enemy))
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.SetDestination(navMeshAgent.transform.position);
            return Status.Success;
        }
        
        return Status.Running;
    }
}

