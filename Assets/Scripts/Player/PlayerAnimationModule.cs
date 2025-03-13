using System;
using UnityEngine;

namespace Player
{
    public class PlayerAnimationModule : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        
        private PlayerMovementModule _playerMovementModule;
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        
        private void Awake()
        {
            _playerMovementModule = GetComponent<PlayerMovementModule>();
        }

        private void Update()
        {
            SetMovementParameters();
        }

        private void SetMovementParameters()
        {
            _animator.SetBool(IsMoving, _playerMovementModule.IsMoving);
            _animator.SetFloat(Horizontal, _playerMovementModule.Horizontal);
            _animator.SetFloat(Vertical, _playerMovementModule.Vertical);
        }
    }
}
