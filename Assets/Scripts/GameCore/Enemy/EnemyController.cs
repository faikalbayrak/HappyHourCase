using System;
using System.Threading.Tasks;
using GameCore.Player;
using GameCore.Projectiles;
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
        [SerializeField] private ParticleSystem burnDamageVfx;
        [SerializeField] private Transform bounceArrowPos;
        [SerializeField] private Transform bounceArrowRootObject;
        
        private HealthController _healthController;
        private HealthBarUI _healthBarUI;
        private LookAt _lookAt;
        
        private IObjectResolver _objectResolver;
        private IGameManagerService _gameManagerService;
        private IObjectPoolService _objectPoolService;
        private IAudioService _audioService;
        private Transform nearestEnemy;
        
        private bool _dependenciesInjected = false;
        private bool isEnabledDot = false;
        private float dotDuration = 3f;
        private float dotDurationMultiplier = 1f;
        private float damageInterval = 1f;
        private float dotTimer = 0f;
        private float nextDamageTime = 0f;
        private bool isEnabledDotTemp = false;

        public bool IsEnabledDot
        {
            get => isEnabledDot;
            set => isEnabledDot = value;
        }

        public float DotDurationMultiplier
        {
            get => dotDurationMultiplier;
            set => dotDurationMultiplier = value;
        }
        
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

        private void LateUpdate()
        {
            if (isEnabledDot)
            {
                burnDamageVfx.Play();
                isEnabledDotTemp = true;
                
                dotTimer += Time.deltaTime;
                
                if (dotTimer >= nextDamageTime)
                {
                    TakeDamage(10);
                    nextDamageTime += damageInterval;
                }
                
                if (dotTimer >= (dotDuration * dotDurationMultiplier))
                {
                    isEnabledDot = false;
                    dotTimer = 0f;
                    nextDamageTime = 0f;
                }
            }
            else
            {
                if(isEnabledDotTemp)
                    burnDamageVfx.Stop();
            }

            if (gameObject.activeSelf)
            {
                if (_gameManagerService != null)
                {
                    LookAtNearestEnemy();
                }
            }
        }

        public void SetObjectResolver(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
            
            if (_objectResolver != null)
            {
                _objectPoolService = _objectResolver.Resolve<IObjectPoolService>();
                _audioService = objectResolver.Resolve<IAudioService>();
            }
        }

        public void SetIGameManagerService(IGameManagerService gameManagerService)
        {
            _gameManagerService = gameManagerService;
        }

        public void OnObjectSpawn()
        {
            _healthController.Heal(100);
            isEnabledDot = false;
            isEnabledDotTemp = false;
            dotTimer = 0f;
            nextDamageTime = 0f;
            dotDurationMultiplier = 1;
            burnDamageVfx.Stop();
        }

        private void ReturnToPool()
        {
            if (_objectPoolService != null)
            {
                Explosion();
                _objectPoolService.ReturnToPool("Enemy", gameObject);
                
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

        private void LookAtNearestEnemy()
        {
            nearestEnemy = _gameManagerService.GetNearestEnemy(transform);
            if (nearestEnemy != null)
            {
                Vector3 direction = nearestEnemy.position - bounceArrowRootObject.position;
                direction.y = 0;
                bounceArrowRootObject.rotation = Quaternion.LookRotation(direction);
                bounceArrowRootObject.GetChild(0).rotation = Quaternion.LookRotation(direction);
            }
        }

        private async void Explosion()
        {
            _audioService.PlayOneShot("Explosion");
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
        
        public void BounceArrow(bool isRaged,int rageBounceCount)
        {
            if (isRaged)
            {
                if(rageBounceCount == 0)
                    return;
            }
            
            GameObject arrow = _objectPoolService.SpawnFromPool("Arrow", bounceArrowPos.position, bounceArrowPos.rotation);
    
            if (nearestEnemy != null && nearestEnemy.gameObject.activeSelf)
            {
                if (arrow != null)
                {
                    Arrow arrowScript = arrow.GetComponent<Arrow>();
                    if (isRaged)
                    {
                        arrowScript.RageBounceCount = rageBounceCount - 1;
                        arrowScript.IsBounced = true;
                        arrowScript.IsRaged = true;
                    }
                    arrowScript.Launch(nearestEnemy.position + new Vector3(0, 0.5f, 0));
                }
            }
        }
    }
}
