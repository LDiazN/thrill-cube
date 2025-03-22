using System;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public class PlayerHealthGUI : MonoBehaviour
{
    #region Inspector Properties

    [Description("Text field where to show player health")]
    [SerializeField]
    private TextMeshProUGUI healthText;

    #endregion

    #region Internal State

    private Player _player;
    public Player Player { get => _player; set => SetPlayer(value); }

    #endregion

    private void Start()
    {
        // Init Health Value
        if (_player != null)
        {
            _player.Health.OnHealthChanged += OnHealthChange;
            UpdateHealthText(_player.Health);
        }
    }

    private void OnEnable()
    {
        _player = FindFirstObjectByType<Player>();
    }

    private void OnDisable()
    {
        if (_player == null)
            return;
        
        _player.Health.OnHealthChanged -= OnHealthChange;
    }

    void OnHealthChange(Health health, Health.Change change)
    {
        UpdateHealthText(health);
    }

    void UpdateHealthText(Health health)
    {
        healthText.text = health.CurrentHealth.ToString();
    }

    public void SetPlayer(Player player)
    {
        if (_player != null)        
            _player.Health.OnHealthChanged -= OnHealthChange;
        
        _player = player;
        _player.Health.OnHealthChanged += OnHealthChange;
    }
}
