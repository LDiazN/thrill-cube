using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindClosestEnemy", story: "Check my [perceptions] to find the closest [enemy]", category: "Action", id: "20b141363c3e7ff5f3ca9661d4eb234c")]
public partial class FindClosestEnemyAction : Action
{
    [SerializeReference] public BlackboardVariable<AIGuide> Perceptions;
    [SerializeReference] public BlackboardVariable<Enemy> Enemy;

    protected override Status OnStart()
    {
        var perceptions = Perceptions.Value;
        var enemy = perceptions.GetClosestEnemy();
        if (!enemy)
        {
            Enemy.Value = null;
            return Status.Failure;
        }

        Enemy.Value = enemy;
        return Status.Success;
    }
}

