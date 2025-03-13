using UnityEngine;

namespace Player
{
    public class PlayerMovementModule : MonoBehaviour
    {
        [SerializeField] private FloatingJoystick _joystick;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _rotationSpeed = 10f;

        private CharacterController _characterController;
        private bool _isMoving = false;
        
        public bool IsMoving => _isMoving;
        public float Horizontal => _joystick.Horizontal;
        public float Vertical => _joystick.Vertical;
        
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            MovePlayer();
            RotatePlayer();
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
    }
}