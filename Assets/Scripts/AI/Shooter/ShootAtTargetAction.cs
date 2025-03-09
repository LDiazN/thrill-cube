using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ShootAtPlayer", story: "I will shoot my [gun] to [player]", category: "Action", id: "d7e90f6bc82d46826eff023d7865616b")]
public partial class ShootAtPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<Gun> Gun;
    [SerializeReference] public BlackboardVariable<Player> Player;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var gun = Gun.Value;
        if (gun.IsOnRecoil)
            return Status.Running;
        
        var player = Player.Value;
        gun.Fire(player.transform.position, GameObject);
        
        return Status.Success;
    }

}

