using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetNavigationSpeed", story: "Set my navigation speed to [navSpeed]", category: "Action", id: "92895f29c9bf367bb94c09e597a045b9")]
public partial class SetNavigationSpeedAction : Action
{
    [SerializeReference] public BlackboardVariable<float> NavSpeed;

    protected override Status OnUpdate()
    {
        var navMeshAgent = GameObject.GetComponent<NavMeshAgent>();
        navMeshAgent.speed = NavSpeed.Value;
        return Status.Success;
    }

}

