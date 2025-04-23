using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ShootEnemy", story: "Shoot my [gun] at [enemy] while in [range] until it dies", category: "Action", id: "3e0b1a533ef403131695931363293fda")]
public partial class ShootEnemyAction : Action
{
    [SerializeReference] public BlackboardVariable<AIGuide> Gun;
    [SerializeReference] public BlackboardVariable<Enemy> Enemy;
    [SerializeReference] public BlackboardVariable<RangeDetector> Range;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var enemy = Enemy.Value;
        var aiGuide = Gun.Value;
        var range = Range.Value;

        var playerLayer = 1 << LayerMask.NameToLayer("Player");
        if (enemy == null || !range.HasLineOfSight(enemy, playerLayer) || !aiGuide.EnemyOnSight())
        {
            aiGuide.ShouldShoot = false;
            return Status.Failure;
        }

        if (enemy.Health.isDead)
        {
            aiGuide.ShouldShoot = false;
            return Status.Success;
        }

        aiGuide.ShouldShoot = true; 
        return Status.Running;
    }

    protected override void OnEnd()
    {
        var aiGuide = Gun.Value;
        aiGuide.ShouldShoot = false;
    }
}

