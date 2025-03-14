using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarUI : MonoBehaviour, IHealthObserver
{
    [SerializeField] private ProgressBarPro _healthBar;
    public void OnHealthChanged(int currentHealth, int maxHealth)
    {
        _healthBar.SetValue((float)currentHealth/maxHealth);
    }
}
