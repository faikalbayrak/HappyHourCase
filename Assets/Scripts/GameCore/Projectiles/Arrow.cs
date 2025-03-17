using System;
using System.Collections;
using System.Threading.Tasks;
using GameCore.Enemy;
using UnityEngine;
using Interfaces;
using Managers;
using VContainer;

namespace GameCore.Projectiles
{
    public class Arrow : MonoBehaviour, IPooledObject
    {
        private IObjectResolver _objectResolver;
        private IGameManagerService _gameManagerService;
        private IObjectPoolService _objectPoolService;
        private Rigidbody _rigidbody;
        private bool _dependenciesInjected = false;
        
        private bool _isLaunched = false;
        private bool _isBurned = false;
        private bool _isBounced = false;
        private bool _isRaged = false;
        private int _ragedBounceCount = 3;
        
        public float launchForce = 20f;
        public float gravityScale = 1f;
        public int damage = 10;

        public bool IsBurned
        {
            get => _isBurned;
            set => _isBurned = value;
        }
        
        public bool IsBounced
        {
            get => _isBounced;
            set => _isBounced = value;
        }
        
        public bool IsRaged
        {
            get => _isRaged;
            set => _isRaged = value;
        }

        public int RageBounceCount
        {
            get => _ragedBounceCount;
            set => _ragedBounceCount = value;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
        }

        private void FixedUpdate()
        {
            if (_isLaunched)
            {
                _rigidbody.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);
            }
        }

        public void SetObjectResolver(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;

            if (_objectResolver != null)
            {
                _objectPoolService = _objectResolver.Resolve<IObjectPoolService>();
                
                _dependenciesInjected = true;
            }
        }

        public void OnObjectSpawn()
        {
            _isLaunched = false;
            // _rigidbody.velocity = Vector3.zero;
            // _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.useGravity = false;
        }

        public void Launch(Vector3 targetPosition)
        {
            _isLaunched = true;
            _rigidbody.useGravity = true;

            Vector3 direction = (targetPosition - transform.position).normalized;
            _rigidbody.velocity = direction * launchForce;

            transform.rotation = Quaternion.LookRotation(_rigidbody.velocity);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Projectile") || other.CompareTag("Wall"))
                return;

            HitEffect();
            
            EnemyController enemy = other.transform.parent.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                enemy.IsEnabledDot = _isBurned;
                enemy.DotDurationMultiplier = _isRaged ? 2 : 1;
                if (_isBounced)
                {
                    enemy.BounceArrow(_isRaged,_ragedBounceCount);
                }
            }

            ReturnToPool();
        }

        private void ReturnToPool()
        {
            _isLaunched = false;
            _isBurned = false;
            _isBounced = false;
            _isRaged = false;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.useGravity = false;
            _ragedBounceCount = 3;
            if (_objectPoolService != null)
            {
                _objectPoolService.ReturnToPool("Arrow", gameObject);
                Debug.Log("Arrow returned to pool.");
            }
            else
            {
                Debug.LogWarning("ObjectPoolService is not assigned.");
            }
        }

        private void HitEffect()
        {
            GameObject hitVfx = _objectPoolService.SpawnFromPool("Hit", transform.position, Quaternion.identity);
            hitVfx.GetComponent<ParticleSystem>().Play();
            hitVfx.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            hitVfx.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
            hitVfx.transform.GetChild(2).GetComponent<ParticleSystem>().Play();
            
            ReturnToPoolWithDelay(hitVfx);
        }

        private async void ReturnToPoolWithDelay(GameObject prefab)
        {
            await Task.Delay(new TimeSpan(0,0,0,0,150));
            _objectPoolService.ReturnToPool("Hit",prefab);
        }
    }
}
