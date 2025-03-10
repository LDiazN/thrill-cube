using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckHurtedInPerceptions", story: "Check my [perceptions] to see if I was damaged by [Player]", category: "Action", id: "3f26dbc35ddf53c12fc2c88036ccc1a3")]
public partial class CheckHurtedInPerceptionsAction : Action
{
    [SerializeReference] public BlackboardVariable<Perceptions> Perceptions;
    [SerializeReference] public BlackboardVariable<Player> Player;
    protected override Status OnStart()
    {
        var perceptions = Perceptions.Value;
        if (perceptions.LastHurted == null)
            return Status.Failure;
        
        var playerComp = perceptions.LastHurted.GetComponent<Player>();
        if (playerComp == null)
            return Status.Failure;
        
        Player.Value = playerComp;
        return Status.Success;
    }
}

