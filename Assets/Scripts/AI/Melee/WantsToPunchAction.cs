using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WantsToPunch", story: "Set my [punchattack] to wanna punch", category: "Action", id: "0f25bbc75b3641a6dffebbc95d29c28b")]
public partial class WantsToPunchAction : Action
{
    [SerializeReference] public BlackboardVariable<PunchAttack> Punchattack;

    protected override Status OnStart()
    {
        
        var punchAttack = Punchattack.Value;
        punchAttack.WantsToPunch = true;
        return Status.Success;
    }
}

