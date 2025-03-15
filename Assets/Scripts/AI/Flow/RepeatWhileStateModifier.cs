using System;
using Unity.Behavior;
using UnityEngine;
using Modifier = Unity.Behavior.Modifier;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RepeatWhileState", story: "Repeat while [currentState] is [targetState]", category: "Flow", id: "d55f42dc39c8b53876ef141cbc052050")]
public partial class RepeatWhileStateModifier : Modifier
{
    [SerializeReference] public BlackboardVariable<ShooterEnemyState> CurrentState;
    [SerializeReference] public BlackboardVariable<ShooterEnemyState> TargetState;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var currentState = CurrentState.Value;
        var targetState = TargetState.Value;
        Status status = Child.CurrentStatus;
        if (status == Status.Failure || status == Status.Success)
        {
            var newStatus = StartNode(Child);
            if (newStatus == Status.Failure || newStatus == Status.Success)
                return Status.Running;
        }
        return Status.Waiting;
    }
}

