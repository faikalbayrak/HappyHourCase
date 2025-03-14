using Interfaces;
using UnityEngine;
using VContainer;

namespace Player
{
    public class PlayerMovementModule : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _rotationSpeed = 10f;
        
        private Transform _cameraFollow;
        private FloatingJoystick _joystick;
        private IObjectResolver _objectResolver;
        private CharacterController _characterController;
        private IGameManagerService _gameManagerService;
        
        private bool _isMoving = false;
        private float _smoothVelocity = 0f;
        private bool depenciesInjected = false;

        public bool IsMoving => _isMoving;

        public float Horizontal
        {
            get
            {
                if (_joystick != null)
                    return _joystick.Horizontal;

                return 0;
            }
        }
        public float Vertical
        {
            get
            {
                if (_joystick != null)
                    return _joystick.Vertical;

                return 0;
            }
        }

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (!depenciesInjected)
                return;
            
            MovePlayer();
            RotatePlayer();
            UpdateCameraZPosition();
        }

        private void MovePlayer()
        {
            Vector3 moveDirection = new Vector3(_joystick.Horizontal, 0f, _joystick.Vertical);

            _isMoving = moveDirection.magnitude > 0;

            Vector3 movement = moveDirection.normalized * _moveSpeed * Time.deltaTime;

            _characterController.Move(movement);
        }

        private void RotatePlayer()
        {
            Vector3 moveDirection = new Vector3(_joystick.Horizontal, 0f, _joystick.Vertical);

            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }
        }

        private void UpdateCameraZPosition()
        {
            float targetZ = Mathf.Clamp(transform.position.z, transform.position.z - 10, transform.position.z);
            
            float newZ = Mathf.SmoothDamp(_cameraFollow.position.z, targetZ, ref _smoothVelocity, 0.2f);
            
            Vector3 newPosition = _cameraFollow.position;
            newPosition.z = newZ;
            _cameraFollow.position = newPosition;
        }

        public void SetObjectResolver(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
            
            if (_objectResolver != null)
            {
                _gameManagerService = _objectResolver.Resolve<IGameManagerService>();
                
                if (_gameManagerService != null)
                {
                    var dep = _gameManagerService.GetPlayerMovementDependencies();

                    _joystick = dep.JoyStick;
                    _cameraFollow = dep.CameraFollow;

                    depenciesInjected = true;
                }
            }
        }
    }
}