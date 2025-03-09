using System.ComponentModel;
using UnityEngine;

public class ShootAttack : MonoBehaviour
{
    #region Inspector Properties

    [Description("Gun used to shoot the player")]
    [SerializeField] private Gun gun;

    #endregion


    public void ShootAt(Vector3 target)
    {
        gun.Fire(target, gameObject);
    }
    
}
