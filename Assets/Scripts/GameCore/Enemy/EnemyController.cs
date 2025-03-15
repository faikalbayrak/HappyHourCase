using Interfaces;
using UnityEngine;
using VContainer;

namespace GameCore.Enemy
{
    public class EnemyController : MonoBehaviour, IPooledObject
    {
        private IObjectResolver _objectResolver;
        private IGameManagerService _gameManagerService;
        private bool _dependenciesInjected = false;
        
        public void SetObjectResolver(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
            
            if (_objectResolver != null)
            {
                _gameManagerService = _objectResolver.Resolve<IGameManagerService>();
                
                if (_gameManagerService != null)
                {
                    _dependenciesInjected = true;
                }
            }
        }

        public void OnObjectSpawn()
        {
            
        }
    }
}
