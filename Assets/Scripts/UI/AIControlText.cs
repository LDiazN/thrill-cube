using System;
using TMPro;
using UnityEngine;

public class AIControlText : MonoBehaviour
{
    #region Inspector Properties

    [SerializeField]
    private TextMeshProUGUI _text;
    
    [SerializeField]
    private Player player;

    #endregion


    private void Reset()
    {
        player = FindFirstObjectByType<Player>();
    }

    private void OnEnable()
    {
        if (player)
            player.OnPlayerControllerChanged += OnPlayerControlledChanged;
    }
    
    private void OnDisable()
    {
        if(player)
            player.OnPlayerControllerChanged -= OnPlayerControlledChanged;
    }

    private void OnPlayerControlledChanged(bool usingAI)
    {
        _text.gameObject.SetActive(usingAI);
    }
}
