using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckKnowledgeArea", story: "Check my [perceptions] to see if someone found [Player]", category: "Action", id: "b8ed6df3068275bb1057c6438758af1f")]
public partial class CheckKnowledgeAreaAction : Action
{
    [SerializeReference] public BlackboardVariable<Perceptions> Perceptions;
    [SerializeReference] public BlackboardVariable<Player> Player;
    protected override Status OnStart()
    {
        var perceptions = Perceptions.Value;

        if (perceptions.knowledgeArea != null && perceptions.knowledgeArea.player != null)
        {
            Player.Value = perceptions.knowledgeArea.player;
            return Status.Success;
        }
        
        return Status.Failure;
    }
}

