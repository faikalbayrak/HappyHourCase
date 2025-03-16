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
        private IObjectPoolService _objectPool;
        private Rigidbody _rigidbody;
        private bool _dependenciesInjected = false;

        public float launchForce = 20f;
        public float gravityScale = 1f;
        public int damage = 10;

        private bool _isLaunched = false;

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
                _objectPool = _objectResolver.Resolve<IObjectPoolService>();
                
                _dependenciesInjected = true;
            }
        }

        public void OnObjectSpawn()
        {
            _isLaunched = false;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
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
            if (other.CompareTag("Player"))
                return;

            EnemyController enemy = other.transform.parent.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            ReturnToPool();
        }

        private void ReturnToPool()
        {
            _isLaunched = false;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.useGravity = false;

            if (_objectPool != null)
            {
                _objectPool.ReturnToPool("Arrow", gameObject);
                Debug.Log("Arrow returned to pool.");
            }
            else
            {
                Debug.LogWarning("ObjectPoolService is not assigned.");
            }
        }
    }
}
