using UnityEngine;

namespace GameCore.Player
{
    public class HealthBarUI : MonoBehaviour, IHealthObserver
    {
        [SerializeField] private ProgressBarPro _healthBar;
        public void OnHealthChanged(int currentHealth, int maxHealth)
        {
            _healthBar.SetValue((float)currentHealth/maxHealth);
        }
    }
}
