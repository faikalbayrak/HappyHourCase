using System;
using System.Collections.Generic;

namespace Player
{
    public class HealthController
    {
        private int _currentHealth;
        private int _maxHealth;
    
        private List<IHealthObserver> _observers = new List<IHealthObserver>();

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;

        public HealthController(int maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
        }
    
        public void TakeDamage(int damage)
        {
            _currentHealth = Math.Max(_currentHealth - damage, 0);
            NotifyObservers();
        }
    
        public void Heal(int amount)
        {
            _currentHealth = Math.Min(_currentHealth + amount, _maxHealth);
            NotifyObservers();
        }
    
        public void AddObserver(IHealthObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }
    
        public void RemoveObserver(IHealthObserver observer)
        {
            if (_observers.Contains(observer))
            {
                _observers.Remove(observer);
            }
        }
    
        private void NotifyObservers()
        {
            foreach (var observer in _observers)
            {
                observer.OnHealthChanged(_currentHealth, _maxHealth);
            }
        }
    }
}