using Interfaces;
using Player;
using UnityEngine;
using Utilities;
using VContainer;

namespace GameCore.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private int _maxHealth = 100;

        private float _permanentAttackSpeedMultiplier = 1f;
        private float _temporaryAttackSpeedMultiplier = 1f;
        private float _attackSpeedTimer = 0f;
        
        private HealthController _healthController;
        private HealthBarUI _healthBarUI;
        private LookAt _lookAt;
        private IGameManagerService _gameManagerService;
        private PlayerMovementModule _playerMovementModule;
        private PlayerAnimationModule _playerAnimationModule;
        private PlayerTargetingModule _playerTargetingModule;
        
        private void Awake()
        {
            _healthController = new HealthController(_maxHealth);
            _healthBarUI = GetComponent<HealthBarUI>();
            _lookAt = GetComponentInChildren<LookAt>();
            
            // Observer addition
            if (_healthBarUI != null)
            {
                _healthController.AddObserver(_healthBarUI);
            }
        }
        
        private void LateUpdate()
        {
            if (_attackSpeedTimer > 0)
            {
                _attackSpeedTimer -= Time.deltaTime;
                
                if (_attackSpeedTimer <= 0)
                {
                    _temporaryAttackSpeedMultiplier = _permanentAttackSpeedMultiplier;
                }
            }
        }

        public void SetObjectResolver(IObjectResolver objectResolver)
        {
            _playerMovementModule = GetComponent<PlayerMovementModule>();
            _playerAnimationModule = GetComponent<PlayerAnimationModule>();
            _playerTargetingModule = GetComponent<PlayerTargetingModule>();
            
            if (objectResolver != null)
            {
                _gameManagerService = objectResolver.Resolve<IGameManagerService>();

                _lookAt.LookAtTransform = _gameManagerService.GetVirtualCamPos();
                
                _playerMovementModule.SetObjectResolver(objectResolver);
                _playerTargetingModule.SetObjectResolver(objectResolver);
                _playerAnimationModule.SetObjectResolver(objectResolver);
            }
        }
        
        public void SetAttackSpeedMultiplier(float multiplier, float duration = 0f)
        {
            if (duration > 0)
            {
                _temporaryAttackSpeedMultiplier = multiplier;
                _attackSpeedTimer = duration;
                Debug.Log($"Temporary attack speed multiplier set to {multiplier} for {duration} seconds.");
            }
            else
            {
                _permanentAttackSpeedMultiplier = multiplier;
                _temporaryAttackSpeedMultiplier = multiplier;
                _attackSpeedTimer = 0f;
                Debug.Log($"Permanent attack speed multiplier set to {multiplier}.");
            }
        }
        
        public float GetAttackSpeedMultiplier()
        {
            return _temporaryAttackSpeedMultiplier;
        }
        
        public void TakeDamage(int damage)
        {
            _healthController.TakeDamage(damage);
        }
    }
}
