using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class PlayerHealthGUI : MonoBehaviour
{
    #region Inspector Properties

    [Description("Character icon to show the player's health")] [SerializeField]
    private GameObject _playerIconPrefab;

    #endregion

    #region Internal State

    private Player _player;
    public Player Player { get => _player; set => SetPlayer(value); }

    #endregion

    private void Start()
    {
        SetPlayer(_player);
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
        UpdateHealthCount(health);
    }

    void UpdateHealthCount(Health health)
    {
        if (health.CurrentHealth > transform.childCount)
        {
            var diff = health.CurrentHealth - transform.childCount;
            for (int i = 0; i < diff; i++)
            {
                Instantiate(_playerIconPrefab, transform);
            }
        }
        else
        {
            var diff = transform.childCount - health.CurrentHealth;
            for (int i = 0; i < diff; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }

    public void SetPlayer(Player player)
    {
        if (_player != null)        
            _player.Health.OnHealthChanged -= OnHealthChange;
        
        _player = player;

        if (!_player)
            return;
        
        _player.Health.OnHealthChanged += OnHealthChange;
        Init(_player);
    }

    private void Init(Player player)
    {
        UpdateHealthCount(player.Health);
    }
}
