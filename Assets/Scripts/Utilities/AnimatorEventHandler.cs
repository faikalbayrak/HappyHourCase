using GameCore.Player;
using Player;
using UnityEngine;

namespace Utilities
{
    public class AnimatorEventHandler : MonoBehaviour
    {
        [SerializeField] private PlayerAnimationModule _playerAnimationModule;
        
        public void AttackEvent()
        {
            _playerAnimationModule.AttackEvent();
        }
    }
}
