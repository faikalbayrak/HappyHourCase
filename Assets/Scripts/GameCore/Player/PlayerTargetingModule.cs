using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using VContainer;

namespace GameCore.Player
{
    public class PlayerTargetingModule : MonoBehaviour
    {
        private IObjectResolver _objectResolver;
        private IGameManagerService _gameManagerService;
        private bool _dependenciesInjected = false;

        public Transform CurrentTarget { get; private set; } = null;

        private void Update()
        {
            if (_dependenciesInjected)
            {
                FindClosestEnemy();
            }
        }
        public void SetObjectResolver(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
            
            if (_objectResolver != null)
            {
                _gameManagerService = _objectResolver.Resolve<IGameManagerService>();
                
                if (_gameManagerService != null)
                {
                    _dependenciesInjected = true;
                }
            }
        }
    
        private void FindClosestEnemy()
        {
            List<GameObject> enemies = _gameManagerService.CreatedEnemies;

            if (enemies == null || enemies.Count == 0)
            {
                CurrentTarget = null;
                return;
            }

            Transform closestEnemy = null;
            float closestDistance = Mathf.Infinity;

            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;

                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy.transform;
                }
            }

            CurrentTarget = closestEnemy;
        }

        private void OnDrawGizmos()
        {
            if (CurrentTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, CurrentTarget.position);
            }
        }
    }
}
