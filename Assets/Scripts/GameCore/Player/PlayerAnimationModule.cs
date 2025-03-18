using System;
using System.Collections.Generic;
using GameCore.Projectiles;
using Interfaces;
using Player;
using UnityEngine;
using VContainer;

namespace GameCore.Player
{
    public class PlayerAnimationModule : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _arrowFirePos;
        [SerializeField] private Transform _arrowFirePos2;
        [SerializeField] private Transform _arrowFirePos3;
        [SerializeField] private Transform _arrowFirePos4;
        
        private IObjectResolver _objectResolver;
        private IGameManagerService _gameManagerService;
        private IObjectPoolService _objectPool;
        private IAudioService _audioService;
        private bool _dependenciesInjected = false;
        
        private PlayerController _playerController;
        private PlayerMovementModule _playerMovementModule;
        private PlayerTargetingModule _playerTargetingModule;

        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        private static readonly int Attack = Animator.StringToHash("IsAttacking");
        private static readonly int AttackSpeed = Animator.StringToHash("AttackSpeed");
        
        private enum PlayerState
        {
            Idle,
            Moving,
            Attacking
        }

        private PlayerState _currentState = PlayerState.Idle;
        

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _playerMovementModule = GetComponent<PlayerMovementModule>();
            _playerTargetingModule = GetComponent<PlayerTargetingModule>();
        }

        private void Update()
        {
            UpdateState();
            HandleState();
        }
        
        public void SetObjectResolver(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
            
            if (_objectResolver != null)
            {
                _gameManagerService = _objectResolver.Resolve<IGameManagerService>();
                _objectPool = _objectResolver.Resolve<IObjectPoolService>();
                _audioService = _objectResolver.Resolve<IAudioService>();
                
                if (_gameManagerService != null)
                {
                    _dependenciesInjected = true;
                }
            }
        }

        private void SetMovementParameters()
        {
            _animator.SetBool(IsMoving, _playerMovementModule.IsMoving);
            _animator.SetFloat(Horizontal, _playerMovementModule.Horizontal);
            _animator.SetFloat(Vertical, _playerMovementModule.Vertical);
        }

        private void SetAttackParameters()
        {
            _animator.SetFloat(AttackSpeed,_playerController.GetAttackSpeedMultiplier());
        }

        private void UpdateState()
        {
            if (_playerMovementModule.IsMoving)
            {
                _currentState = PlayerState.Moving;
            }
            else if (_playerTargetingModule.CurrentTarget != null &&
                     Vector3.Distance(transform.position, _playerTargetingModule.CurrentTarget.position) < 6f)
            {
                _currentState = PlayerState.Attacking;
            }
            else
            {
                _currentState = PlayerState.Idle;
            }
        }

        private void HandleState()
        {
            switch (_currentState)
            {
                case PlayerState.Idle:
                    HandleIdleState();
                    break;
                case PlayerState.Moving:
                    HandleMovingState();
                    break;
                case PlayerState.Attacking:
                    HandleAttackState();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleIdleState()
        {
            SetMovementParameters();
            
            _animator.SetBool(IsMoving, false);
            _animator.SetBool(Attack, false);
        }

        private void HandleMovingState()
        {
            SetMovementParameters();
            
            _animator.SetBool(IsMoving, true);
            _animator.SetBool(Attack, false);
        }

        private void HandleAttackState()
        {
            SetAttackParameters();
            
            _animator.SetBool(IsMoving, false);
            _animator.SetBool(Attack, true);
            
            if (_playerTargetingModule.CurrentTarget != null)
            {
                Vector3 direction = _playerTargetingModule.CurrentTarget.position - transform.position;
                direction.y = 0;

                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 20f);
                }
            }
        }
        
        public void AttackEvent()
        {
            if (_playerTargetingModule.CurrentTarget == null)
            {
                Debug.LogWarning("No target to attack.");
                return;
            }

            List<Transform> firePositions = new List<Transform> { _arrowFirePos };

            if (_playerController.IsDoubleArrowActivated)
            {
                firePositions.Add(_arrowFirePos2);
                
                if (_playerController.IsRageActivated)
                {
                    firePositions.Add(_arrowFirePos3);
                    firePositions.Add(_arrowFirePos4);
                }
            }
            
            foreach (var firePos in firePositions)
            {
                SpawnAndLaunchArrow(firePos);
            }
        }

        private void SpawnAndLaunchArrow(Transform firePos)
        {
            _audioService.PlayOneShot("ThrowArrow");
            GameObject arrow = _objectPool.SpawnFromPool("Arrow", firePos.position, firePos.rotation);
            if (arrow != null && _playerTargetingModule.CurrentTarget.gameObject.activeSelf)
            {
                Arrow arrowScript = arrow.GetComponent<Arrow>();
                arrowScript.IsBurned = _playerController.IsBurnDamageActivated;
                arrowScript.IsBounced = _playerController.IsBounceArrowActivated;
                arrowScript.IsRaged = _playerController.IsRageActivated;
                arrowScript.Launch(_playerTargetingModule.CurrentTarget.position + new Vector3(0, 0.5f, 0));
            }
        }

        public void BowDrawEvent()
        {
            _audioService.PlayOneShot("DrawBow");
        }
    }
}