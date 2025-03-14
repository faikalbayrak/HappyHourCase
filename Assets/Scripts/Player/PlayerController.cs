using Interfaces;
using UnityEngine;
using Utilities;
using VContainer;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private int _maxHealth = 100;
        
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
            }
        }
    }
}
