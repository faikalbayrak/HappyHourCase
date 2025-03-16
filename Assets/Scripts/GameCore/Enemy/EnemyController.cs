using System.Threading.Tasks;
using GameCore.Player;
using Interfaces;
using Player;
using UnityEngine;
using Utilities;
using VContainer;

namespace GameCore.Enemy
{
    public class EnemyController : MonoBehaviour, IPooledObject, IHealthObserver
    {
        [SerializeField] private int _maxHealth = 100;
        
        private HealthController _healthController;
        private HealthBarUI _healthBarUI;
        private LookAt _lookAt;
        
        private IObjectResolver _objectResolver;
        private IGameManagerService _gameManagerService;
        private ISceneLoadService _sceneLoadService;
        private IObjectPoolService _objectPoolService;
        private bool _dependenciesInjected = false;
        
        private void Awake()
        {
            _healthController = new HealthController(_maxHealth);
            _healthBarUI = GetComponent<HealthBarUI>();
            _lookAt = GetComponentInChildren<LookAt>();
            
            // Observer addition
            if (_healthBarUI != null)
            {
                _healthController.AddObserver(_healthBarUI);
                _healthController.AddObserver(this);
            }
        }
        
        public void SetObjectResolver(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
            
            if (_objectResolver != null)
            {
                _sceneLoadService = _objectResolver.Resolve<ISceneLoadService>();
                _objectPoolService = _objectResolver.Resolve<IObjectPoolService>();
                
                _sceneLoadService.OnGameSceneLoaded += OnGameSceneLoaded;
            }
        }

        public void SetIGameManagerService(IGameManagerService gameManagerService)
        {
            _gameManagerService = gameManagerService;
        }

        private void OnGameSceneLoaded()
        {
            
        }

        public void OnObjectSpawn()
        {
            
        }

        private void ReturnToPool()
        {
            if (_objectPoolService != null)
            {
                Explosion();
                _objectPoolService.ReturnToPool("Enemy", gameObject);
                _healthController.Heal(100);
                Debug.Log("Enemy returned to pool.");
            }
            else
            {
                Debug.LogWarning("ObjectPoolService is not assigned.");
            }
        }
        
        public void TakeDamage(int damage)
        {
            _healthController.TakeDamage(damage);
        }

        public void OnHealthChanged(int currentHealth, int maxHealth)
        {
            if (currentHealth <= 0)
                Die();
        }

        private void Die()
        {
            ReturnToPool();
        }

        private async void Explosion()
        {
            GameObject explosiveVfx = _objectPoolService.SpawnFromPool("Explosive", transform.position + new Vector3(0,0.5f,0), Quaternion.identity);
            
            _gameManagerService.ExecuteCinemachineImpulse();
            explosiveVfx.GetComponent<ParticleSystem>().Play();
            explosiveVfx.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            explosiveVfx.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
            explosiveVfx.transform.GetChild(2).GetComponent<ParticleSystem>().Play();
            explosiveVfx.transform.GetChild(3).GetComponent<ParticleSystem>().Play();
            explosiveVfx.transform.GetChild(4).GetComponent<ParticleSystem>().Play();

            await Task.Delay(2000);
            
            _objectPoolService.ReturnToPool("Explosive",explosiveVfx);
        }
    }
}
